using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public interface IEventSourcedRepository<T> where T : IEventSourced
	{
		/// <summary>
		/// Tries to retrieve the event sourced entity.
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>The hydrated entity, or null if it does not exist.</returns>
		T Find(Guid id);

		/// <summary>
		/// Retrieves the event sourced entity.
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>The hydrated entity</returns>
		/// <exception cref="AggregateNotFoundException">If the entity is not found.</exception>
		T Get(Guid id);

		/// <summary>
		/// Saves the event sourced entity.
		/// </summary>
		/// <param name="eventSourced">The entity.</param>
		/// <param name="correlationId">A correlation id to use when publishing events.</param>
		void Save(T eventSourced, string correlationId);

		/// <summary>
		/// Saves the event sourced entity asynchronously.
		/// </summary>
		/// <param name="eventSourced">The entity.</param>
		/// <param name="correlationId">A correlation id to use when publishing events.</param>
		Task SaveAsync(T eventSourced, string correlationId);
	}

	/// <summary>
	/// Defines a repository of <see cref="IEventSourced"/> entities.
	/// </summary>
	/// <typeparam name="T">The entity type to persist.</typeparam>
	public class EventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced
	{
		// Note: Could potentially use DataAnnotations to get a friendly/unique name in case of collisions between BCs, instead of the type's name.
		private static readonly string sourceType = typeof(T).Name;

		private readonly IEventStore eventStore;
		private readonly IEventStoreNotifier notifier;
		private readonly ITextSerializer serializer;
		private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> entityFactory;
		private readonly Func<Guid, ISnapshot, IEnumerable<IVersionedEvent>, T> originatorEntityFactory;
		private readonly IMetadataProvider metadataProvider;
		private readonly ObjectCache cache;
		private readonly Action<T> cacheSnapshotIfApplicable;
		private readonly Func<Guid, Tuple<ISnapshot, DateTime?>> getSnapshotFromCache;
		private readonly Action<Guid> markCacheAsStale;

		public AzureEventSourcedRepository(IEventStore eventStore, IEventStoreNotifier notifier,
			ITextSerializer serializer, IMetadataProvider metadataProvider, ObjectCache cache)
		{
			this.eventStore = eventStore;
			this.notifier = notifier;
			this.serializer = serializer;
			this.metadataProvider = metadataProvider;
			this.cache = cache;

			// TODO: could be replaced with a compiled lambda to make it more performant
			var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
			if (constructor == null)
			{
				throw new InvalidCastException(
					"Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
			}
			entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });

			if (typeof(ISnapshotGenerator).IsAssignableFrom(typeof(T)) && this.cache != null)
			{
				// TODO: could be replaced with a compiled lambda to make it more performant
				var snapshotConstructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(ISnapshot), typeof(IEnumerable<IVersionedEvent>) });
				if (snapshotConstructor == null)
				{
					throw new InvalidCastException(
						"Type T must have a constructor with the following signature: .ctor(Guid, ISnapshot, IEnumerable<IVersionedEvent>)");
				}
				originatorEntityFactory = (id, snapshot, events) => (T)snapshotConstructor.Invoke(new object[] { id, snapshot, events });
				cacheSnapshotIfApplicable = (T originator) =>
					{
						string key = GetPartitionKey(originator.Id);
						var snapshot = ((ISnapshotGenerator)originator).SaveToSnapshot();
						this.cache.Set(
							key,
							new Tuple<ISnapshot, DateTime?>(snapshot, DateTime.UtcNow),
							new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30) });
					};
				getSnapshotFromCache = id => (Tuple<ISnapshot, DateTime?>)this.cache.Get(GetPartitionKey(id));
				markCacheAsStale = id =>
				{
					var key = GetPartitionKey(id);
					var item = (Tuple<ISnapshot, DateTime?>)this.cache.Get(key);
					if (item != null && item.Item2.HasValue)
					{
						item = new Tuple<ISnapshot, DateTime?>(item.Item1, null);
						this.cache.Set(
							key,
							item,
							new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30) });
					}
				};
			}
			else
			{
				// if no cache object or is not a cache originator, then no-op
				cacheSnapshotIfApplicable = o => { };
				getSnapshotFromCache = id => { return null; };
				markCacheAsStale = id => { };
			}
		}

		public T Find(Guid id)
		{
			var cachedSnapshot = getSnapshotFromCache(id);
			if (cachedSnapshot != null && cachedSnapshot.Item1 != null)
			{
				// NOTE: if we had a guarantee that this is running in a single process, there is
				// no need to check if there are new events after the cached version.
				IEnumerable<IVersionedEvent> deserialized;
				if (!cachedSnapshot.Item2.HasValue || cachedSnapshot.Item2.Value < DateTime.UtcNow.AddSeconds(-1))
				{
					deserialized = eventStore.Load(GetPartitionKey(id), cachedSnapshot.Item1.Version + 1).Select(Deserialize);
				}
				else
				{
					// if the cache entry was updated in the last seconds, then there is a high possibility that it is not stale
					// (because we typically have a single writer for high contention aggregates). This is why we optimistically avoid
					// getting the new events from the EventStore since the last Snapshot was created. In the low probable case
					// where we get an exception on save, then we mark the cache item as stale so when the command gets
					// reprocessed, this time we get the new events from the EventStore.
					deserialized = Enumerable.Empty<IVersionedEvent>();
				}

				return originatorEntityFactory.Invoke(id, cachedSnapshot.Item1, deserialized);
			}
			else
			{
				var partitionKey = GetPartitionKey(id);
				var deserialized = eventStore.Load(partitionKey, 0)
					.Select(Deserialize)
					.AsCachedAnyEnumerable();

				if (deserialized.Any())
				{
					return entityFactory.Invoke(id, deserialized);
				}
			}

			return null;
		}


		public T Get(Guid id)
		{
			var entity = Find(id);
			if (entity == null)
			{
				throw new EntityNotFoundException(id, sourceType);
			}

			return entity;
		}

		/// <summary>
		/// This method saves the events created from a command to azure
		/// and using EventStoreBusPublisher to process later
		/// </summary>
		/// <param name="eventSourced"></param>
		/// <param name="correlationId"></param>
		public void Save(T eventSourced, string correlationId)
		{
			// TODO: guarantee that only incremental versions of the event are stored
			var events = eventSourced.Events.ToArray();
			var serialized = events.Select(e => Serialize(e, correlationId));

			var partitionKey = GetPartitionKey(eventSourced.Id);
			try
			{
				eventStore.Save(partitionKey, serialized);
			}
			catch
			{
				markCacheAsStale(eventSourced.Id);
				throw;
			}
			//the event is saved in azure storage before it is processed
			//TODO Notify EventProcessor to generate viewModels based on events saved in azure table storage
			notifier.Notify(partitionKey);
			cacheSnapshotIfApplicable.Invoke(eventSourced);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSourced"></param>
		/// <param name="correlationId"></param>
		/// <returns></returns>
		public Task SaveAsync(T eventSourced, string correlationId)
		{
			// TODO: guarantee that only incremental versions of the event are stored
			var events = eventSourced.Events.ToArray();
			var serialized = events.Select(e => Serialize(e, correlationId));

			var partitionKey = GetPartitionKey(eventSourced.Id);
			try
			{
				return eventStore.SaveAsync(partitionKey, serialized).ContinueWith((task =>
				{
					//TODO change to service bus publisher
					notifier.Notify(partitionKey);
					cacheSnapshotIfApplicable.Invoke(eventSourced);
				}));
			}
			catch
			{
				markCacheAsStale(eventSourced.Id);
				throw;
			}
		}

		private string GetPartitionKey(Guid id)
		{
			return sourceType + "_" + id;
		}

		private EventData Serialize(IVersionedEvent e, string correlationId)
		{
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, e);
				var metadata = metadataProvider.GetMetadata(e);
				return new EventData
				{
					Version = e.Version,
					SourceId = e.SourceId.ToString(),
					Payload = writer.ToString(),
					SourceType = sourceType,
					CorrelationId = correlationId,
					// Standard metadata
					AssemblyName = metadata.TryGetValue(StandardMetadata.AssemblyName),
					Namespace = metadata.TryGetValue(StandardMetadata.Namespace),
					TypeName = metadata.TryGetValue(StandardMetadata.TypeName),
					FullName = metadata.TryGetValue(StandardMetadata.FullName)
				};
			}
		}

		private IVersionedEvent Deserialize(EventData @event)
		{
			using (var reader = new StringReader(@event.Payload))
			{
				return (IVersionedEvent)serializer.Deserialize(reader);
			}
		}
	}
	
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	

	/// <summary>
	/// Defines a repository of <see cref="IEventSourced"/> entities.
	/// </summary>
	/// <typeparam name="T">The entity type to persist.</typeparam>
	public class EventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced
	{
		// Note: Could potentially use DataAnnotations to get a friendly/unique name in case of collisions between BCs, instead of the type's name.
		private readonly string sourceType = typeof(T).Name;

		private readonly IEventStore eventStore;
		private readonly IEventStoreNotifier notifier;
		private readonly ITextSerializer serializer;
		private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> entityFactory;
		private readonly IMetadataProvider metadataProvider;
		private readonly Action<T> cacheSnapshotIfApplicable;
		private readonly Action<Guid> markCacheAsStale;

		public string SourceType
		{
			get
			{
				return sourceType;
			}
		}

		public EventSourcedRepository(IEventStore eventStore, IEventStoreNotifier notifier,
			ITextSerializer serializer, IMetadataProvider metadataProvider)
		{
			this.eventStore = eventStore;
			this.notifier = notifier;
			this.serializer = serializer;
			this.metadataProvider = metadataProvider;

			// TODO: could be replaced with a compiled lambda to make it more performant
			var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
			if (constructor == null)
			{
				throw new InvalidCastException(
					"Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
			}
			entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });

		}

		public T Find(Guid id)
		{
			//var deserialized = eventStore.LoadEvents(id)
			//	.Select(Deserialize)
			//	.AsCachedAnyEnumerable();

			//if (deserialized.Any())
			//{
			//	return entityFactory.Invoke(id, deserialized);
			//}

			return null;
		}

		public T Get(Guid id)
		{
			var entity = Find(id);
			if (entity == null)
			{
				throw new AggregateNotFoundException(id, SourceType);
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

			eventStore.SaveEvents(eventSourced.Events, serialized);

			//the event is saved in azure storage before it is processed
			//TODO Notify EventProcessor to generate viewModels based on events saved in azure table storage
			//notifier.Notify(partitionKey);
			//????? TODO
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

			return eventStore.SaveEvents(serialized, serialized).ContinueWith((task =>
			{
				//TODO change to service bus publisher
				//notifier.Notify(partitionKey);
				cacheSnapshotIfApplicable.Invoke(eventSourced);
			}));
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
					SourceType = SourceType,
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

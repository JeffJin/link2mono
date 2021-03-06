using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private readonly IEventBus eventBus;
		private readonly ITextSerializer serializer;
		private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> entityFactory;
		private readonly IMetadataProvider metadataProvider;

		public string SourceType
		{
			get
			{
				return sourceType;
			}
		}

		public EventSourcedRepository(IEventStore eventStore, IEventBus eventBus, ITextSerializer serializer,
		                              IMetadataProvider metadataProvider)
		{
			this.eventStore = eventStore;
			this.eventBus = eventBus;
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

		public Task<T> Find(Guid id)
		{
			var eventSourced = eventStore.LoadEvents(id).ContinueWith(events =>
			{
				var deserialized = events.Result.Select(Deserialize);

				if (deserialized.Any())
				{
					return entityFactory.Invoke(id, deserialized);
				}
				else
				{
					return null;
				}
			});

			return eventSourced;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSourced">Aggregate Object</param>
		/// <param name="correlationId">Command ID</param>
		/// <returns></returns>
		public Task Save(T eventSourced, Guid correlationId)
		{
			Debug.WriteLine("EventSourcedRepository.Save");
			// TODO: guarantee that only incremental versions of the event are stored
			var events = eventSourced.Events.ToArray();
			var serialized = events.Select(e => Serialize(e, correlationId));

			return eventStore.SaveEvents(serialized).ContinueWith(task =>
			{
				if (task.Result)
				{
					return eventBus.Publish(serialized);
				}
				return Task.FromResult(false);
				//TODO
				//cacheSnapshotIfApplicable.Invoke(eventSourced);
			});
		}


		private EventData Serialize(IVersionedEvent e, Guid correlationId)
		{
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, e);
				var metadata = metadataProvider.GetMetadata(e);
				return new EventData
				{
					Version = e.Version,
					SourceId = e.SourceId,
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

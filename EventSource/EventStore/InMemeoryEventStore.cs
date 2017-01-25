using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appointments.EventHandlers;
using EventSource;

namespace Appointments.EventHandlers
{
	public class InMemeoryEventStore: IEventStore
	{
		private List<IEvent> storage = new List<IEvent>();

		public Task<bool> DeleteEvents(Guid sourceId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<IEvent>> LoadEvents(Guid correlationId)
		{
			var data = storage.Where(e => e.CorrelationId == correlationId);
			return Task.FromResult(data);
		}

		public Task<bool> SaveEvents(IEnumerable<IEvent> events)
		{
			var eventTasks = new List<Task>();
			foreach (var evt in events)
			{
				eventTasks.Add(SaveEvent(evt));
			}
			return (System.Threading.Tasks.Task<bool>)Task.WhenAll(eventTasks);
		}

		public Task<bool> SaveEvent(IEvent eventData)
		{
			IEvent data = storage.Find(e => e.SourceId == eventData.SourceId);
			if (data != null)
			{
				throw new InvalidOperationException(String.Format("event data with same ID {0} is not allowed", eventData.SourceId));
			}
			else
			{
				storage.Add(eventData);
				return Task.FromResult(true);
			}
		}

		public Task<IEvent> LoadEvent(Guid eventId)
		{
			IEvent data = storage.Find(e => e.SourceId == eventId);
			return Task.FromResult(data);
		}
	}
}
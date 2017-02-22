using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Appointments.EventHandlers;
using EventSource;

namespace Appointments.EventHandlers
{
    public class InMemeoryEventStore: IEventStore
	{
		private List<EventData> storage = new List<EventData>();

		public Task<bool> DeleteEvents(Guid sourceId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<EventData>> LoadEvents(Guid correlationId)
		{
			var data = storage.Where(e => e.CorrelationId == correlationId);
			return Task.FromResult(data);
		}

		public Task<bool> SaveEvents(IEnumerable<EventData> events)
		{
			try
			{
				var eventTasks = new List<Task>();
				foreach (var evt in events)
				{
					eventTasks.Add(SaveEvent(evt));
				}
				Task.WaitAll(eventTasks.ToArray());
			}
			catch (InvalidOperationException ex)
			{
				Trace.WriteLine(ex.Message);
				return Task.FromResult(false);
			}

			return Task.FromResult(true);
		}

		public Task<bool> SaveEvent(EventData eventData)
		{
			EventData data = storage.Find(e => e.SourceId == eventData.SourceId);
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

		public Task<EventData> LoadEvent(Guid eventId)
		{
			EventData data = storage.Find(e => e.SourceId == eventId);
			return Task.FromResult(data);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appointments.EventHandlers;
using EventSource;

namespace Appointments.EventHandlers
{
	public class InMemeoryEventStore: IEventStore
	{
		private Dictionary<Guid, EventData> storage = new Dictionary<Guid, EventData>();

		public Task DeleteEvents(Guid sourceId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<EventData>> LoadEvents(Guid sourceId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<EventData>> SaveEvents(IEnumerable<EventData> events)
		{
			//if (storage.TryGetValue(model.Id, out result))
			//{
			//	throw new InvalidOperationException(String.Format("read model with same ID {0} is not allowed", model.Id));
			//}
			//else
			//{
			//	storage[model.Id] = model;
			//	return Task.FromResult(true);
			//}
			throw new NotImplementedException();
		}

		public Task<EventData> LoadEvent(Guid eventId)
		{
			var data = storage[eventId];
			return Task.FromResult(data);
		}
	}
}
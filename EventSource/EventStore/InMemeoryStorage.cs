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

		public Task Get(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task DeleteEvents(Guid rootId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<EventData>> LoadEvents(Guid rootId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<IEvent>> SaveEvents(Guid rootId, IEnumerable<EventData> events)
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

		public Task<IEvent> SaveEvent(Guid rootId, EventData evt)
		{

			throw new NotImplementedException();
			//if (storage.TryGetValue(evt.Id, out result))
			//{
			//	throw new InvalidOperationException(String.Format("read model with same ID {0} is not allowed", model.Id));
			//}
			//else
			//{
			//	storage[model.Id] = model;
			//	return Task.FromResult(true);
			//}
		}
	}
}
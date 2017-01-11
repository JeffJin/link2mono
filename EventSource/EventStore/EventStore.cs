using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	//SQL, InMemory or ElasticSearch for read model persistence

	public class EventStore : IEventStore
	{
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
			throw new NotImplementedException();
		}
	}
}
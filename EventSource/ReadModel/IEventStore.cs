using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	//SQL, InMemory or ElasticSearch for read model persistence
	public interface IEventStore
	{
		Task SaveEvents(Guid rootId, IEnumerable<EventData> events); //CancellationToken cancellationToken

		Task<IEnumerable<EventData>> LoadEvents(Guid rootId); //CancellationToken cancellationToken

		Task DeleteEvents(Guid rootId); //CancellationToken cancellationToken
		
	}
}
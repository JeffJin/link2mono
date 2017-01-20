using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	//SQL, InMemory or ElasticSearch for read model persistence
	public interface IEventStore
	{
		Task<IEnumerable<EventData>> SaveEvents(IEnumerable<EventData> events); //CancellationToken cancellationToken

		Task<EventData> LoadEvent(Guid eventId); //CancellationToken cancellationToken

		Task<IEnumerable<EventData>> LoadEvents(Guid sourceId); //CancellationToken cancellationToken

		Task DeleteEvents(Guid sourceId); //CancellationToken cancellationToken
		
	}
}
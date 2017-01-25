using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	//SQL, InMemory or ElasticSearch for read model persistence
	public interface IEventStore
	{
		Task<bool> SaveEvents(IEnumerable<IEvent> events); //CancellationToken cancellationToken

		Task<bool> SaveEvent(IEvent eventData); //CancellationToken cancellationToken

		Task<IEvent> LoadEvent(Guid eventId); //CancellationToken cancellationToken

		Task<IEnumerable<IEvent>> LoadEvents(Guid correlationId); //CancellationToken cancellationToken

		Task<bool> DeleteEvents(Guid correlationId); //CancellationToken cancellationToken
		
	}
}
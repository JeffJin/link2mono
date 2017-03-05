using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	//SQL, InMemory or ElasticSearch for read model persistence
	public interface IEventStore
	{
		Task<bool> SaveEvents(IEnumerable<EventData> events); //CancellationToken cancellationToken

		Task<bool> SaveEvent(EventData eventData); //CancellationToken cancellationToken

		Task<IEnumerable<EventData>> LoadEvents(Guid correlationId); //CancellationToken cancellationToken

        //Load all events (TODO: need to load partially or create snapshot)
        IEnumerable<EventData> LoadEvents(); //CancellationToken cancellationToken

        //events should not be deleted for replay purpose
//		Task<bool> DeleteEvents(Guid correlationId); //CancellationToken cancellationToken
		
	}

    public interface ISqlEventStore : IEventStore
    {
        DbConnection GetConnection();

        EventData LoadNextEvent(DbConnection connection, DbTransaction transaction);
        void MarkEventAsProcessed(EventData eventData, DbTransaction transaction);
    }
}
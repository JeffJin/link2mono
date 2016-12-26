using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointments.Events
{
	//SQL, InMemory or ElasticSearch for read model persistence
	public interface IEventPersistence
	{
		Task SaveEvents(Guid id, CancellationToken cancellationToken);

		Task LoadEvents(Guid id, CancellationToken cancellationToken);

		Task DeleteEvents(Guid id, CancellationToken cancellationToken);
		
	}
}
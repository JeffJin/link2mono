using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSource;

namespace Appointments.EventHandlers
{
    public class SqlEventStore : IEventStore
    {
        public Task<bool> SaveEvents(IEnumerable<EventData> events)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEvent(EventData eventData)
        {
            throw new NotImplementedException();
        }

        public Task<EventData> LoadEvent(Guid eventId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventData>> LoadEvents(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEvents(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }
}
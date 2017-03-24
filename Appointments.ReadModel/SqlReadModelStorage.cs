using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.Dto;
using EventSource;

namespace Appointments.ReadModel
{
    public class SqlReadModelStorage : IReadModelStorage<AppointmentReadModel>
    {
        private readonly Func<ReadModelContext> _contextFactory;

        public SqlReadModelStorage(Func<ReadModelContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        Task<IEnumerable<AppointmentReadModel>> IReadModelStorage<AppointmentReadModel>.GetAll(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task Save(AppointmentReadModel model)
        {
            throw new NotImplementedException();
        }

        public Task Get(Guid id)
        {
            throw new NotImplementedException();
        }
        
    }
}

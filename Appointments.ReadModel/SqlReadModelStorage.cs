using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public Task<List<AppointmentReadModel>> GetAll(int pageIndex, int pageSize)
        {
            using (var context = this._contextFactory.Invoke())
            {
                var results = context.Appointments.OrderBy(appt => appt.CreatedOn).Skip(pageIndex * pageSize).Take(pageSize);
            
                return Task.FromResult(results.ToList());
            }
        }

        public Task Save(AppointmentReadModel model)
        {
            using (var context = this._contextFactory.Invoke())
            {
                context.Appointments.Add(model);
                var result = context.SaveChanges();
                return Task.FromResult(result);
            }
        }

        public Task<AppointmentReadModel> Get(Guid id)
        {
            using (var context = this._contextFactory.Invoke())
            {
                var appt = context.Appointments.SingleOrDefault(apt => apt.Id == id);
                return Task.FromResult(appt);
            }
        }
    }
}

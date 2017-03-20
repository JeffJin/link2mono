using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventSource;

namespace Appointments.EventHandlers
{
    public class SqlReadModelStorage<T> : IReadModelStorage<T> where T: IReadModel
    {
        private string connectionString;
        private string tableName;

        public SqlReadModelStorage(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
        }

        public Task Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();

        }

        public Task Save(T model)
        {
            throw new NotImplementedException();
        }
    }
}
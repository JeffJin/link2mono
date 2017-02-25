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
        private SqlConnectionFactory connectionFactory;
        private string insertQuery;
        private string readQuery;
        private string tableName;
        private string deleteQuery;

        public SqlReadModelStorage(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;

            this.connectionFactory = new SqlConnectionFactory(connectionString);
            this.insertQuery =
                string.Format(
                    "INSERT INTO {0} (SourceId, Version, SourceType, Payload, CorrelationId, AssemblyName, Namespace, FullName, TypeName) " +
                    "VALUES (@SourceId, @Version, @SourceType, @Payload, @CorrelationId, @AssemblyName, @Namespace, @FullName, @TypeName)",
                    tableName);

            this.readQuery =
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"SELECT
                    {0}.[SourceId] AS [SourceId], 
                    {0}.[Version] AS [Version], 
                    {0}.[SourceType] AS [SourceType],
                    {0}.[Payload] AS [Payload],
                    {0}.[CorrelationId] AS [CorrelationId],
                    {0}.[AssemblyName] AS [AssemblyName],
                    {0}.[Namespace] AS [Namespace],
                    {0}.[FullName] AS [FullName],
                    {0}.[TypeName] AS [TypeName]
                    FROM {0} WITH (UPDLOCK, READPAST)
                    ORDER BY {0}.[Version] ASC",
                    tableName);

            this.deleteQuery =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "DELETE FROM {0} WHERE SourceId = @SourceId",
                    tableName);
        }

        private Dictionary<Guid, T> storage = new Dictionary<Guid, T>();

        public Task Get(Guid id)
        {
            T result;
            if (storage.TryGetValue(id, out result))
            {
                return Task.FromResult(result);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize)
        {
            var results = storage.Values.Skip(pageIndex * pageSize).Take(pageSize);
            return Task.FromResult(results);
        }

        public Task Save(T model)
        {
            T result;
            if (storage.TryGetValue(model.Id, out result))
            {
                throw new InvalidOperationException(String.Format("read model with same ID {0} is not allowed", model.Id));
            }
            else
            {
                storage[model.Id] = model;
                return Task.FromResult(true);
            }
        }
    }
}
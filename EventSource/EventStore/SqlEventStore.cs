using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventSource;

namespace Appointments.EventHandlers
{
    public class SqlEventStore : IEventStore
    {
        private string connectionString;
        private SqlConnectionFactory connectionFactory;
        private string insertQuery;
        private string readQuery;
        private string tableName;
        private string deleteQuery  ;

        public SqlEventStore(string connectionString, string tableName)
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

        private Task<int> InsertEvent(EventData evt, SqlCommand command)
        {
            command.CommandText = insertQuery;
            command.CommandType = CommandType.Text;
            command.Parameters.Add("@SourceId", SqlDbType.UniqueIdentifier).Value = evt.SourceId;
            command.Parameters.Add("@Version", SqlDbType.Int).Value = evt.Version;
            command.Parameters.Add("@SourceType", SqlDbType.NVarChar).Value = evt.SourceType;
            command.Parameters.Add("@Payload", SqlDbType.NVarChar).Value = evt.Payload;
            command.Parameters.Add("@CorrelationId", SqlDbType.UniqueIdentifier).Value = evt.CorrelationId;

            command.Parameters.Add("@AssemblyName", SqlDbType.NVarChar).Value = evt.AssemblyName;
            command.Parameters.Add("@Namespace", SqlDbType.NVarChar).Value = evt.Namespace;
            command.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = evt.FullName;
            command.Parameters.Add("@TypeName", SqlDbType.NVarChar).Value = evt.TypeName;

            return command.ExecuteNonQueryAsync();
        }

        private Task<bool> InsertEvent(EventData evt, DbConnection connection)
        {
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                var task = InsertEvent(evt, command);

                return task.ContinueWith((countTask) =>
                {
                    connection.Dispose();
                    return countTask.Result > 0;
                });
            }
        }

    
        public Task<bool> SaveEvents(IEnumerable<EventData> events)
        {
            DbConnection connection = this.connectionFactory.CreateConnection(this.connectionString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = (SqlCommand)connection.CreateCommand())
                {
                    command.Transaction = (SqlTransaction)transaction;
                    try
                    {
                        foreach (var evt in events)
                        {
                            command.Parameters.Clear();

                            var task = InsertEvent(evt, command);
                            if (task.Result == 0)
                            {
                                //'handled as needed, 
                                //' but this snippet will throw an exception to force a rollback
                                throw new InvalidProgramException();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Task.FromResult(false);
                    }
                }
            }
            return Task.FromResult(true);
        }

        public Task<bool> SaveEvent(EventData eventData)
        {
            DbConnection connection = this.connectionFactory.CreateConnection(this.connectionString);
            return connection.OpenAsync().ContinueWith((task) => InsertEvent(eventData, connection).Result);
        }

        public Task<EventData> LoadEvent(Guid eventId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventData>> LoadEvents(Guid sourceId)
        {
            DbConnection connection = this.connectionFactory.CreateConnection(this.connectionString);
            connection.Open();
            var list = new List<EventData>();
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = this.readQuery;
                ((SqlCommand)command).Parameters.Add("@SourceId", SqlDbType.UniqueIdentifier).Value = sourceId;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var payload = (string)reader["Payload"];
                        var version = (int)reader["Version"];
                        var sourceType = (string)reader["SourceType"];
                        var correlationId = (Guid)reader["CorrelationId"];
                        var assemblyName = (string)reader["AssemblyName"];
                        var nameSpace = (string)reader["Namespace"];
                        var fullname = (string)reader["FullName"];
                        var typename = (string)reader["TypeName"];

                        var evtData = new EventData()
                        {
                            SourceId = sourceId,
                            Payload = payload,
                            Version = version,
                            SourceType = sourceType,
                            CorrelationId = correlationId,
                            AssemblyName = assemblyName,
                            Namespace = nameSpace,
                            FullName = fullname,
                            TypeName = typename
                        };
                        list.Add(evtData);
                    }
                }

                return Task.FromResult(list.AsEnumerable());
            }
        }

        public Task<bool> DeleteEvents(Guid correlationId)
        {
            DbConnection connection = this.connectionFactory.CreateConnection(this.connectionString);
            connection.Open();
            using (var command = (SqlCommand) connection.CreateCommand())
            {
                command.CommandText = insertQuery;
                command.CommandType = CommandType.Text;

                command.Parameters.Add("@CorrelationId", SqlDbType.UniqueIdentifier).Value = correlationId;

                command.ExecuteNonQuery();
            }

            return Task.FromResult(true);
        }
    }
}
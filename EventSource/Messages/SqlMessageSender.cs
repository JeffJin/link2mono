using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Transactions;

namespace EventSource
{
	//this is generic message sender to send messages to a queue(RabbitMQ, Azure Queue or database)
	public class SqlMessageSender : IMessageSender
	{
		private readonly IDbConnectionFactory connectionFactory;
		private readonly string connectionString;
		private readonly string insertQuery;

		public SqlMessageSender(string connectionString, string tableName)
		{
            this.connectionString = connectionString;
            this.connectionFactory = new SqlConnectionFactory(connectionString);
            insertQuery =
                string.Format(
                    "INSERT INTO {0} (Id, Body, DeliveryDate, CorrelationId) VALUES (@Id, @Body, @DeliveryDate, @CorrelationId)",
                    tableName);
        }

		/// <summary>
		/// Sends the specified message.
		/// </summary>
		public Task Send(Message message)
		{
			using (DbConnection connection = this.connectionFactory.CreateConnection(this.connectionString))
			{
				connection.Open();

				return InsertMessage(message, connection);
			}
		}

		/// <summary>
		/// Sends a batch of messages.
		/// </summary>
		public Task Send(IEnumerable<Message> messages)
		{
			var task = Task.Factory.StartNew(() =>
			{
				var tasks = new List<Task>();
				using (var scope = new TransactionScope(TransactionScopeOption.Required))
				{
					using (var connection = connectionFactory.CreateConnection(this.connectionString))
					{
						connection.Open();
						foreach (var message in messages)
						{
							tasks.Add(InsertMessage(message, connection));
						}
					}
					Task.WaitAll(tasks.ToArray());
					scope.Complete();
				}
			});
			return task;
		}

		[SuppressMessage("Microsoft.Security",
			"CA2100:Review SQL queries for security vulnerabilities", Justification = "Does not contain user input.")]
		private Task InsertMessage(Message message, DbConnection connection)
		{
			using (var command = (SqlCommand)connection.CreateCommand())
			{
				command.CommandText = insertQuery;
				command.CommandType = CommandType.Text;

				command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = message.Id;
				command.Parameters.Add("@Body", SqlDbType.NVarChar).Value = message.Body;
				command.Parameters.Add("@DeliveryDate", SqlDbType.DateTime).Value = message.DeliveryDate.HasValue
					? (object)message.DeliveryDate.Value
					: DateTime.MinValue;
				command.Parameters.Add("@CorrelationId", SqlDbType.NVarChar).Value = (object)message.CorrelationId ??
																					 string.Empty;

				return command.ExecuteNonQueryAsync();
			}
		}
	}

}

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
		private readonly string name;
		private readonly string insertQuery;

		public SqlMessageSender(IDbConnectionFactory connectionFactory, string name, string tableName)
		{
			this.connectionFactory = connectionFactory;
			this.name = name;
			insertQuery =
				string.Format(
					"INSERT INTO {0} (Body, DeliveryDate, CorrelationId) VALUES (@Body, @DeliveryDate, @CorrelationId)",
					tableName);
		}

		/// <summary>
		/// Sends the specified message.
		/// </summary>
		public Task Send(Message message)
		{
			using (DbConnection connection = connectionFactory.CreateConnection(name))
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
					using (var connection = connectionFactory.CreateConnection(name))
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

				command.Parameters.Add("@Body", SqlDbType.NVarChar).Value = message.Body;
				command.Parameters.Add("@DeliveryDate", SqlDbType.DateTime).Value = message.DeliveryDate.HasValue
					? (object)message.DeliveryDate.Value
					: DBNull.Value;
				command.Parameters.Add("@CorrelationId", SqlDbType.NVarChar).Value = (object)message.CorrelationId ??
																					 DBNull.Value;

				return command.ExecuteNonQueryAsync();
			}
		}
	}

}

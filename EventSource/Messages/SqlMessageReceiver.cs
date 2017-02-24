using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	public class SqlMessageReceiver: IMessageReceiver, IDisposable
	{
		private readonly IDbConnectionFactory connectionFactory;
		private readonly string connectionString;
		private readonly string readQuery;
		private readonly string deleteQuery;
		private readonly TimeSpan pollDelay;
		private readonly object lockObject = new object();
		private CancellationTokenSource cancellationSource;

		public SqlMessageReceiver(string connectionString, string tableName)
            : this(connectionString, tableName, TimeSpan.FromMilliseconds(200))
        {
		}

		public SqlMessageReceiver(string connectionString, string tableName, TimeSpan pollDelay)
		{
            this.connectionFactory = new SqlConnectionFactory(connectionString);
            this.connectionString = connectionString;
			this.pollDelay = pollDelay;

			this.readQuery =
				string.Format(
					CultureInfo.InvariantCulture,
					@"SELECT TOP (1) 
                    {0}.[Id] AS [Id], 
                    {0}.[Body] AS [Body], 
                    {0}.[DeliveryDate] AS [DeliveryDate],
                    {0}.[CorrelationId] AS [CorrelationId]
                    FROM {0} WITH (UPDLOCK, READPAST)
                    WHERE ({0}.[DeliveryDate] IS NULL) OR ({0}.[DeliveryDate] <= @CurrentDate)
                    ORDER BY {0}.[Id] ASC",
					tableName);
			this.deleteQuery =
				string.Format(
				   CultureInfo.InvariantCulture,
				   "DELETE FROM {0} WHERE Id = @Id",
				   tableName);
		}

		private Action<Message> MessageReceived;

		public void Start(Action<Message> processMessage)
		{
			lock (this.lockObject)
			{
				if (this.cancellationSource == null)
				{
					this.MessageReceived = processMessage;
					this.cancellationSource = new CancellationTokenSource();
					Task.Factory.StartNew(
						() => this.ReceiveMessages(this.cancellationSource.Token),
						this.cancellationSource.Token,
						TaskCreationOptions.LongRunning,
						TaskScheduler.Current);
				}
			}
		}

		public void Stop()
		{
			lock (this.lockObject)
			{
				using (this.cancellationSource)
				{
					if (this.cancellationSource != null)
					{
						this.cancellationSource.Cancel();
						this.cancellationSource = null;
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Stop();
		}

		~SqlMessageReceiver()
		{
			Dispose(false);
		}

		/// <summary>
		/// Receives the messages in an endless loop.
		/// </summary>
		private void ReceiveMessages(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (!this.ReceiveMessage())
				{
					Thread.Sleep(this.pollDelay);
				}
			}
		}

		protected bool ReceiveMessage()
		{
			using (var connection = this.connectionFactory.CreateConnection(this.connectionString))
			{
				var currentDate = GetCurrentDate();

				connection.Open();
				using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
				{
					try
					{
						Guid messageId = Guid.Empty;
						Message message = null;

						using (var command = connection.CreateCommand())
						{
							command.Transaction = transaction;
							command.CommandType = CommandType.Text;
							command.CommandText = this.readQuery;
                            //TODO Only retrieve current day's records?
                            ((SqlCommand)command).Parameters.Add("@CurrentDate", SqlDbType.DateTime).Value = currentDate;

							using (var reader = command.ExecuteReader())
							{
								if (!reader.Read())
								{
									return false;
								}

								var body = (string)reader["Body"];
								var deliveryDateValue = reader["DeliveryDate"];
								var deliveryDate = deliveryDateValue == DBNull.Value ? (DateTime?)null : new DateTime?((DateTime)deliveryDateValue);
								var correlationId = (string)reader["CorrelationId"];

								message = new Message(body, deliveryDate, correlationId);
								messageId = (Guid)(reader["Id"]);
							    message.Id = messageId;
							}
                        }

						this.MessageReceived(message);

						using (var command = connection.CreateCommand())
						{
							command.Transaction = transaction;
							command.CommandType = CommandType.Text;
							command.CommandText = this.deleteQuery;
							((SqlCommand)command).Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = messageId;

							command.ExecuteNonQuery();
						}

						transaction.Commit();
					}
					catch (Exception)
					{
						try
						{
							transaction.Rollback();
						}
						catch
						{
						}
						throw;
					}
				}
			}

			return true;
		}

		protected virtual DateTime GetCurrentDate()
		{
			return DateTime.UtcNow;
		}
	}

}

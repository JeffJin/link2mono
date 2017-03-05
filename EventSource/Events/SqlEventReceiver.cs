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
	public class SqlEventReceiver: IEventReceiver
	{
	    private readonly ISqlEventStore _eventStore;
	    private readonly TimeSpan _pollDelay;
		private readonly TimeSpan pollDelay;
		private readonly object lockObject = new object();
		private CancellationTokenSource cancellationSource;

		public SqlEventReceiver(ISqlEventStore eventStore)
            : this(eventStore, TimeSpan.FromMilliseconds(200))
        {
		}

		public SqlEventReceiver(ISqlEventStore eventStore, TimeSpan pollDelay)
		{
		    _eventStore = eventStore;
		    _pollDelay = pollDelay;
		}

	    private Action<EventData> MessageReceived;

		public void Start(Action<EventData> processMessage)
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

		~SqlEventReceiver()
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
			using (var connection = this._eventStore.GetConnection())
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
				{
					try
					{
					    var eventData = this._eventStore.LoadNextEvent(connection, transaction);

                        this.MessageReceived(eventData);

					    this._eventStore.MarkEventAsProcessed(eventData, transaction);

						transaction.Commit();
					}
					catch (Exception ex)
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

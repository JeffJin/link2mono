using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	public class InMemoryMessageReceiver : IMessageReceiver, IDisposable
	{
		private readonly string name;
		private readonly string readQuery;
		private readonly string deleteQuery;
		private readonly TimeSpan pollDelay;
		private readonly object lockObject = new object();
		private CancellationTokenSource cancellationSource;

		public InMemoryMessageReceiver()
			: this(TimeSpan.FromMilliseconds(100))
		{
		}

		public InMemoryMessageReceiver(TimeSpan pollDelay)
		{
			this.pollDelay = pollDelay;
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived = (sender, args) => { };

		public void Start()
		{
			lock (this.lockObject)
			{
				if (this.cancellationSource == null)
				{
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

		~InMemoryMessageReceiver()
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
			if (InMemoryMessageStore.MessageQueue.Peek() != null)
			{
				Message message = InMemoryMessageStore.MessageQueue.Dequeue();
				this.MessageReceived(this, new MessageReceivedEventArgs(message));
				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual DateTime GetCurrentDate()
		{
			return DateTime.UtcNow;
		}
	}

}

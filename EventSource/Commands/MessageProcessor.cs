using System;

namespace EventSource
{
	public abstract class MessageProcessor
	{
		private const int MaxProcessingRetries = 5;
		private bool disposed;
		private bool started = false;
		private readonly IMessageReceiver receiver;
		private readonly ITextSerializer serializer;
		private readonly object lockObject = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageProcessor"/> class.
		/// </summary>
		protected MessageProcessor(IMessageReceiver receiver, ITextSerializer serializer)
		{
			this.receiver = receiver;
			this.serializer = serializer;
		}

		protected ITextSerializer Serializer { get { return this.serializer; } }

		/// <summary>
		/// Starts the listener.
		/// </summary>
		public virtual void Start()
		{
			ThrowIfDisposed();
			lock (this.lockObject)
			{
				if (!this.started)
				{
					this.receiver.Start();
					this.started = true;
				}
			}
		}

		/// <summary>
		/// Stops the listener.
		/// </summary>
		public virtual void Stop()
		{
			lock (this.lockObject)
			{
				if (this.started)
				{
					this.receiver.Stop();
					this.started = false;
				}
			}
		}

		/// <summary>
		/// Disposes the resources used by the processor.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Processes the message.
		/// </summary>
		/// <param name="payload">The typed message payload.</param>
		protected abstract void ProcessMessage(object payload);

		/// <summary>
		/// Disposes the resources used by the processor.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.Stop();
					this.disposed = true;

					using (this.receiver as IDisposable)
					{
						// Dispose receiver if it's disposable.
					}
				}
			}
		}

		~MessageProcessor()
		{
			Dispose(false);
		}

        private void ThrowIfDisposed()
		{
			if (this.disposed)
				throw new ObjectDisposedException("MessageProcessor");
		}


	}
}
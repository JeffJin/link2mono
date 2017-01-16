using System;
using System.Diagnostics;
using System.IO;

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
		/// Processes the message.
		/// </summary>
		/// <param name="payload">The typed message payload.</param>
		protected abstract void ProcessMessage(object payload);

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
					this.receiver.Start(OnMessageReceived);
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

		private void OnMessageReceived(Message message)
		{
			Trace.WriteLine(new string('-', 100));

			try
			{
				var body = Deserialize(message.Body);

				ProcessMessage(body);

				Trace.WriteLine(new string('-', 100));
			}
			catch (Exception e)
			{
				// NOTE: we catch ANY exceptions as this is for local 
				// development/debugging. The Windows Azure implementation 
				// supports retries and dead-lettering, which would 
				// be totally overkill for this alternative debug-only implementation.
				Trace.TraceError("An exception happened while processing message through handler/s:\r\n{0}", e);
				Trace.TraceWarning("Error will be ignored and message receiving will continue.");
			}
		}

		protected object Deserialize(string serializedPayload)
		{
			using (var reader = new StringReader(serializedPayload))
			{
				return this.serializer.Deserialize(reader);
			}
		}

		protected string Serialize(object payload)
		{
			using (var writer = new StringWriter())
			{
				this.serializer.Serialize(writer, payload);
				return writer.ToString();
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
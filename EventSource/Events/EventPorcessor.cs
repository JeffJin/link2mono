using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventProcessor : IDisposable
	{
		private readonly IEventDispatcher dispatcher;
        private const int MaxProcessingRetries = 5;
        private bool disposed;
        private bool started = false;
        private readonly IMessageReceiver receiver;
        private readonly ITextSerializer serializer;
        private readonly object lockObject = new object();

        public EventProcessor(IMessageReceiver receiver, ITextSerializer serializer,
		                      IEventDispatcher dispatcher)
		{
            this.receiver = receiver;
            this.serializer = serializer;
            this.dispatcher = dispatcher;
		}

        public void Register(IEventHandler eventHandler)
		{
			this.dispatcher.Register(eventHandler);
		}

		/// <summary>
		/// Send the specified command and corrsponding command handlers will generate related events 
		/// and save them to RabbitMQ or any other persistent storage(queue).
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="payload">Event.</param>
		private void ProcessMessage(object payload)
		{
			Debug.WriteLine("EventPorcessor.ProcessMessage - " + payload.ToString());
			
			var message = (IEvent)payload;
			if (message != null)
			{
				this.dispatcher.ProcessEvent(message);
			}
		}

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
            Debug.WriteLine(new string('-', 100));

            try
            {
                using (var reader = new StringReader(message.Body))
                {
                    var body = this.serializer.Deserialize(reader);

                    this.ProcessMessage(body);

                    Debug.WriteLine("MessageProcessor.OnMessageReceived - " + message.ToString());
                }
                //TODO Deserialization fails

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

        /// <summary>
        /// Disposes the resources used by the processor.
        /// </summary>
        public void Dispose(bool disposing)
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

        ~EventProcessor()
        {
            Dispose(false);
        }

	    public void Dispose()
	    {
	        Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException("EventProcessor");
        }


    }

}

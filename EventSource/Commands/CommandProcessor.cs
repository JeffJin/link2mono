using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public class CommandProcessor: IDisposable
	{
	    private const int MaxProcessingRetries = 5;
	    private bool disposed;
	    private bool started = false;
	    private readonly IMessageReceiver receiver;
	    private readonly ITextSerializer serializer;
	    private readonly object lockObject = new object();
        private readonly ICommandDispatcher dispatcher;

	    protected ITextSerializer Serializer { get { return this.serializer; } }

        protected CommandProcessor(IMessageReceiver receiver, ITextSerializer serializer)
	    {
	        this.receiver = receiver;
	        this.serializer = serializer;
	    }


        public CommandProcessor(IMessageReceiver receiver, ITextSerializer serializer, ICommandDispatcher dispatcher): this(receiver, serializer)
		{
			this.dispatcher = dispatcher;
		}

		public void Register(ICommandHandler commandHandler)
		{
			this.dispatcher.Register(commandHandler);
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

	    private void OnMessageReceived(Message message)
	    {
	        Debug.WriteLine(new string('-', 100));

	        try
	        {
	            using (var reader = new StringReader(message.Body))
	            {
	                var cmd = (ICommand)this.serializer.Deserialize(reader);

	                this.dispatcher.ProcessCommand(cmd);

                    Debug.WriteLine("CommandProcessor.OnMessageReceived - " + message.ToString());
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

	    ~CommandProcessor()
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

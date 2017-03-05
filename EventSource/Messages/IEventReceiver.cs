using System;

namespace EventSource
{
    public interface IEventReceiver: IDisposable
    {
        /// <summary>
        /// Stops the listener.
        /// </summary>
        void Stop();

        /// <summary>
        /// Starts the listener.
        /// </summary>
        void Start(Action<EventData> processMessage);
    }
}
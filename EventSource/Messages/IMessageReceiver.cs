using System;

namespace EventSource
{
	//TODO send the messages and notify the handlers
	
	public interface IMessageReceiver
	{
		/// <summary>
		/// Stops the listener.
		/// </summary>
		void Stop();

		/// <summary>
		/// Starts the listener.
		/// </summary>
		void Start(Action<Message> processMessage);
	}
	
	
}

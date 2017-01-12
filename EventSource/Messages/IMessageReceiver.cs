using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO send the messages and notify the handlers
	
	public interface IMessageReceiver
	{
		/// <summary>
		/// Starts the listener.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the listener.
		/// </summary>
		void Stop();
	}

	
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public interface IEventDispatcher
	{
		/// <summary>
		/// Registers the specified event handler.
		/// </summary>
		void Register(IEventHandler eventHandler);

		/// <summary>
		/// Processes the message by calling the registered handler.
		/// </summary>
		bool ProcessEvent(IEvent payload);
	}

	
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventPorcessor
	{
		readonly IEventDispatcher dispatcher;

		public EventPorcessor(IEventDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public bool Publish(IEnumerable<IEvent> events)
		{
			bool result = true;

			foreach (var evt in events)
			{
				result &= Publish(evt);
			}

			return result;
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
		/// <param name="evt">Event.</param>
		public bool Publish(IEvent evt)
		{
			return this.dispatcher.ProcessEvent(evt);
		}
	}

}

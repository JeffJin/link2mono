using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventPorcessor: MessageProcessor
	{
		readonly IEventDispatcher dispatcher;

		public EventPorcessor(IMessageReceiver receiver, ITextSerializer serializer,
		                      IEventDispatcher dispatcher): base(receiver, serializer)
		{
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
		protected override void ProcessMessage(object payload)
		{
			this.dispatcher.ProcessEvent((IEvent)payload);
		}
	}

}

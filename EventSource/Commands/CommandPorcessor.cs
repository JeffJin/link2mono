using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventSource
{

	public class CommandPorcessor: MessageProcessor
	{
		readonly ICommandDispatcher dispatcher;

		public CommandPorcessor(IMessageReceiver receiver, ITextSerializer serializer, ICommandDispatcher dispatcher): 
				base(receiver, serializer)
		{
			this.dispatcher = dispatcher;
		}

		public void Register(ICommandHandler commandHandler)
		{
			this.dispatcher.Register(commandHandler);
		}

		/// <summary>
		/// Processes the message by calling the registered handler.
		/// </summary>
		protected override void ProcessMessage(object payload)
		{
			this.dispatcher.ProcessCommand((ICommand)payload);
		}
	}

}

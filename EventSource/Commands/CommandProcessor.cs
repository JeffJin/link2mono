using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventSource
{

	public class CommandProcessor: MessageProcessor
	{
		readonly ICommandDispatcher dispatcher;

		public CommandProcessor(IMessageReceiver receiver, ITextSerializer serializer, ICommandDispatcher dispatcher): 
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
			Debug.WriteLine("CommandProcessor.ProcessMessage - " + payload.ToString());
			
			ICommand message = (ICommand)payload;
			if (message != null)
			{
				this.dispatcher.ProcessCommand(message);
			}
		}
	}

}

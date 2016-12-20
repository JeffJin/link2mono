using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{
	public class CommandBus: ICommandBus
	{
		readonly ICommandDispatcher dispatcher;

		public CommandBus(ICommandDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public bool Send(IEnumerable<ICommand> commands)
		{
			bool result = true;

			foreach(var cmd in commands){
				result &= Send(cmd);
			}

			return result;
		}

		/// <summary>
		/// Send the specified command and corrsponding command handlers will generate related events 
		/// and save them to RabbitMQ or any other persistent storage(queue).
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="command">Command.</param>
		public bool Send(ICommand command)
		{
			return this.dispatcher.ProcessCommand(command);
		}
	}

	
}

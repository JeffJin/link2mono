using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public class CommandPorcessor
	{
		readonly ICommandDispatcher dispatcher;

		public CommandPorcessor(ICommandDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public bool Publish(IEnumerable<ICommand> commands)
		{
			bool result = true;

			foreach (var cmd in commands)
			{
				result &= Publish(cmd);
			}

			return result;
		}

		public void Register(ICommandHandler commandHandler)
		{
			this.dispatcher.Register(commandHandler);
		}

		/// <summary>
		/// Send the specified command and corrsponding command handlers will generate related events 
		/// and save them to RabbitMQ or any other persistent storage(queue).
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="command">Command.</param>
		public bool Publish(ICommand command)
		{
			return this.dispatcher.ProcessCommand(command);
		}
	}

}

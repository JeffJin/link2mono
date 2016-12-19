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

		public bool Send(ICommand command)
		{
			//TODO command
			return this.dispatcher.ProcessCommand(command);
		}
	}

	
}

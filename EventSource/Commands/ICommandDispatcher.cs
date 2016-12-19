using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{

	public interface ICommandDispatcher
	{
		/// <summary>
		/// Registers the specified command handler.
		/// </summary>
		void Register(ICommandHandler commandHandler);

		/// <summary>
		/// Processes the message by calling the registered handler with trace.
		/// </summary>
		bool ProcessCommand(string traceIdentifier, ICommand payload, string messageId, string correlationId);

		/// <summary>
		/// Processes the message by calling the registered handler.
		/// </summary>
		bool ProcessCommand(ICommand payload);
	}

	
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO send the messages and notify the handlers
	public interface IMessageSender
	{
		/// <summary>
		/// Sends the specified message, and invokes correspnding handlers to process the messages
		/// </summary>
		Task Send(Message message);

		/// <summary>
		/// Sends a batch of messages.
		/// </summary>
		Task Send(IEnumerable<Message> messages);
	}
}

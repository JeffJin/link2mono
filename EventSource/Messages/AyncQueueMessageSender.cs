using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO send the messages and notify the handlers
	public class AyncQueueMessageSender : IMessageSender
	{
		public Task Send(IEnumerable<Message> messages)
		{
			throw new NotImplementedException();
		}

		public Task Send(Message message)
		{
			throw new NotImplementedException();
		}
	}
	
}

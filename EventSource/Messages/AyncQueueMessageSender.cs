using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO RabbitMQ client to send messages
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

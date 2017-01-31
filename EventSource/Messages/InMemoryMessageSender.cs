using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO send the messages and notify the handlers

	public class InMemoryMessageSender : IMessageSender
	{
		public Task Send(IEnumerable<Message> messages)
		{
			var tasks = messages.Select(x => Send(x));

			return Task.WhenAll(tasks);
		}

		public Task Send(Message message)
		{
			InMemoryMessageStore.Instance.MessageQueue.Enqueue(message);

			return Task.FromResult(0);
		}
	}

}

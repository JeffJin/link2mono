using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	//TODO send the messages and notify the handlers

	public class AyncQueueMessageReceiver : IMessageReceiver
	{
		public void Start(Action<Message> action)
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;

namespace EventSource
{
	public static class InMemoryMessageStore
	{
		public static readonly Queue<Message> MessageQueue;

		static InMemoryMessageStore()
		{
			MessageQueue = new Queue<Message>();
		}

	}
}
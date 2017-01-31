using System;
using System.Collections.Generic;

namespace EventSource
{
	public class InMemoryMessageStore
	{
		public readonly Queue<Message> MessageQueue;

		private static InMemoryMessageStore instance;
		
		private InMemoryMessageStore() { 
			MessageQueue = new Queue<Message>();
		}
		
		public static InMemoryMessageStore Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new InMemoryMessageStore();
				}
				return instance;
			}
		}
	}
}
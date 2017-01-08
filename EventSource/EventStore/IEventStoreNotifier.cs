using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{
	public interface IEventStoreNotifier
	{
		/// <summary>
		/// Notifies the publisher that there are new pending events in the specified partitionKey.
		/// </summary>
		/// <param name="partitionKey">The partition key or session ID.</param>
		void Notify(string partitionKey);
	}
	
}

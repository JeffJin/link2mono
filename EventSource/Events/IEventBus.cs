using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{
	public interface IEventBus
	{
		Task Publish(IEvent result);
		Task Publish(IEnumerable<IEvent> result);
	}
}
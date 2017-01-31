using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{
	public interface IEventBus
	{
		Task Publish(EventData result);
		Task Publish(IEnumerable<EventData> result);
	}
}
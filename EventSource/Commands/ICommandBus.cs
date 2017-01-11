using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{
	public interface ICommandBus
	{
		Task Publish(ICommand command);
		Task Publish(IEnumerable<ICommand> commands);
	}
}
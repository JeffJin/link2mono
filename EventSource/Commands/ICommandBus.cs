using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSource
{
	public interface ICommandBus
	{
		bool Send(ICommand command);
		bool Send(IEnumerable<ICommand> commands);
	}
}
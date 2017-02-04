using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSource
{

	public class CommandDispatcher : ICommandDispatcher
	{
		private Dictionary<Type, ICommandHandler> handlers = new Dictionary<Type, ICommandHandler>();

		/// <summary>
		/// Processes the message by calling the registered handler.
		/// </summary>
		public bool ProcessCommand(ICommand payload)
		{
			Console.WriteLine("CommandDispatcher ProcessCommand");
			
			var commandType = payload.GetType();
			ICommandHandler handler = null;

			if (handlers.TryGetValue(commandType, out handler))
			{
				((dynamic)handler).Handle((dynamic)payload);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Registers the specified command handler in an assembly.
		/// </summary>
		public void Register(Assembly assembly)
		{
			//var commandHandlerTypes = assembly
			//	.GetTypes()
			//	.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler)));

			//foreach (var commandHandlerType in commandHandlerTypes)
			//{
			//	var t = commandHandlerType;
			//	if (t.IsAbstract)
			//	{
			//		continue;
			//	}
			//	Register(commandHandlerType);
			//}


		}

		/// <summary>
		/// Registers the specified command handler.
		/// </summary>
		public void Register(ICommandHandler commandHandler)
		{
			var genericHandler = typeof(ICommandHandler<>);
			var supportedCommandTypes = commandHandler.GetType()
				.GetInterfaces()
				.Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
				.Select(iface => iface.GetGenericArguments()[0])
				.ToList();

			if (handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
				throw new ArgumentException("The command handled by the received handler already has a registered handler.");

			// Register this handler for each of he handled types.
			foreach (var commandType in supportedCommandTypes)
			{
				handlers.Add(commandType, commandHandler);
			}
		}

	}
}

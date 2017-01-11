using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventDispatcher : IEventDispatcher
	{
		private Dictionary<Type, IEventHandler> handlers = new Dictionary<Type, IEventHandler>();

		/// <summary>
		/// Processes the message by calling the registered handler.
		/// </summary>
		public bool ProcessEvent(IEvent payload)
		{
			var eventType = payload.GetType();
			IEventHandler handler = null;

			if (handlers.TryGetValue(eventType, out handler))
			{
				((dynamic)handler).Handle(payload);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Registers the specified command handler.
		/// </summary>
		public void Register(IEventHandler eventHandler)
		{
			var genericHandler = typeof(IEventHandler<>);
			var supportedEventTypes = eventHandler.GetType()
				.GetInterfaces()
				.Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
				.Select(iface => iface.GetGenericArguments()[0])
				.ToList();

			if (handlers.Keys.Any(registeredType => supportedEventTypes.Contains(registeredType)))
				throw new ArgumentException("The command handled by the received handler already has a registered handler.");

			// Register this handler for each of he handled types.
			foreach (var eventType in supportedEventTypes)
			{
				handlers.Add(eventType, eventHandler);
			}
		}

	}
}

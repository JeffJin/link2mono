using System;
using System.Collections.Generic;

namespace EventSource
{

	/// <summary>
	/// Base class for event sourced entities that implements <see cref="IEventSourced"/>. 
	/// </summary>
	/// <remarks>
	/// <see cref="IEventSourced"/> entities do not require the use of <see cref="EventSourced"/>, but this class contains some common 
	/// useful functionality related to versions and rehydration from past events.
	/// </remarks>
	public abstract class EventSourced : IEventSourced
	{
		private readonly Dictionary<Type, Action<IVersionedEvent>> handlers = new Dictionary<Type, Action<IVersionedEvent>>();
		private readonly List<IVersionedEvent> pendingEvents = new List<IVersionedEvent>();

		private readonly Guid id;
		private int version = -1;

		protected EventSourced(Guid id)
		{
			this.id = id;
		}

		public Guid Id
		{
			get { return id; }
		}

		/// <summary>
		/// Gets the entity's version. As the entity is being updated and events being generated, the version is incremented.
		/// </summary>
		public int Version
		{
			get { return version; }
			protected set { version = value; }
		}

		/// <summary>
		/// Gets the collection of new events since the entity was loaded, as a consequence of command handling.
		/// </summary>
		public IEnumerable<IVersionedEvent> Events
		{
			get { return pendingEvents; }
		}

		/// <summary>
		/// Configures a handler for an event. 
		/// </summary>
		protected void Handles<TEvent>(Action<TEvent> handler)
			where TEvent : IEvent
		{
			handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
		}

		/// <summary>
		/// Replay the history
		/// </summary>
		/// <param name="pastEvents"></param>
		protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
		{
			foreach (var e in pastEvents)
			{
				handlers[e.GetType()].Invoke(e);
				version = e.Version;
			}
		}

		protected void Update(VersionedEvent e)
		{
			e.SourceId = Id;
			e.Version = version + 1;
			handlers[e.GetType()].Invoke(e);
			version = e.Version;
			pendingEvents.Add(e);
		}
	}

	
}

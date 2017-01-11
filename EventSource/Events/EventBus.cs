using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventBus : IEventBus
	{
		readonly IMessageSender sender;
		readonly ITextSerializer serializer;

		public EventBus(IMessageSender sender, ITextSerializer serializer)
		{
			this.serializer = serializer;
			this.sender = sender;
		}

		public Task Publish(IEnumerable<IEvent> events)
		{
			var tasks = new List<Task>();

			foreach (var evt in events)
			{
				tasks.Add(Publish(evt));
			}

			return Task
				.Factory
				.ContinueWhenAll(tasks.ToArray(), finalTask => {}
			);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="evt">Event.</param>
		public Task Publish(IEvent evt)
		{
			Message message = BuildMessage(evt);
			return this.sender.Send(message);
		}

		private Message BuildMessage(IEvent evt)
		{
			// TODO: should use the Command ID as a unique constraint when storing it.
			using (var payloadWriter = new StringWriter())
			{
				this.serializer.Serialize(payloadWriter, evt);
				return new Message(payloadWriter.ToString(), null);
			}
		}
		
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		public Task Publish(IEnumerable<EventData> events)
		{
			Debug.WriteLine("Publish Events");
            var messages = events.Select(evt => BuildMessage(evt));
            return this.sender.Send(messages);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="evt">Event.</param>
		public Task Publish(EventData evt)
		{
			Message message = BuildMessage(evt);
			return this.sender.Send(message);
		}

		private Message BuildMessage(EventData evt)
		{
			// TODO: should use the Command ID as a unique constraint when storing it.
			using (var payloadWriter = new StringWriter())
			{
				this.serializer.Serialize(payloadWriter, evt);
				return new Message(payloadWriter.ToString());
			}
		}
		
	}
}
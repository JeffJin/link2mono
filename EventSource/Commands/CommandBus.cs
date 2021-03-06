﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventSource
{
	public class CommandBus: ICommandBus
	{
		readonly IMessageSender sender;
		readonly ITextSerializer serializer;

		public CommandBus(IMessageSender sender, ITextSerializer serializer)
		{
			this.serializer = serializer;
			this.sender = sender;
		}

		public Task Publish(IEnumerable<ICommand> commands)
		{
			Debug.WriteLine("Publish Commands");
		    var messages = commands.Select(BuildMessage);
		    return this.sender.Send(messages);
		}

		/// <summary>
		/// Send the specified command and corrsponding command handlers will generate related events 
		/// and save them to RabbitMQ or any other persistent storage(queue).
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="command">Command.</param>
		public Task Publish(ICommand command)
		{
			Message message = BuildMessage(command);
			return this.sender.Send(message);
		}

        private Message BuildMessage(ICommand command)
		{
			// TODO: should use the Command ID as a unique constraint when storing it.
			using (var payloadWriter = new StringWriter())
			{
				this.serializer.Serialize(payloadWriter, command);
				return new Message(payloadWriter.ToString(), DateTime.UtcNow, command.Id.ToString());
			}
		}
	}

	

	

}

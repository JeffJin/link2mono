using System;
using System.IO;
using System.Threading.Tasks;
using Appointments.Aggregates;
using Appointments.Commands;
using Appointments.Dto;
using Appointments.EventHandlers;
using EventSource;
using NUnit.Framework;

namespace Appointments.Services.Tests
{
    [Category("integration")]
	public class MakeAppointmentTests
	{
		[Test]
		public void TestMakeAppointment()
		{
			//Setup
			IMessageSender sender = new InMemoryMessageSender();
			ITextSerializer serializer = new JsonTextSerializer();
			IEventStore eventStore = new InMemeoryEventStore();

			IEventBus eventBus = new EventBus(sender, serializer);
			IMetadataProvider metaProvider = new StandardMetadataProvider();
			IReadModelStorage<AppointmentReadModel> readModelStorage = new InMemeoryStorage<AppointmentReadModel>();

			IEventSourcedRepository<AppointmentAggregate> repo = new
				EventSourcedRepository<AppointmentAggregate>(eventStore, eventBus, serializer, metaProvider);
			ICommandDispatcher cmdDispatcher = new CommandDispatcher();
			cmdDispatcher.Register(new AppointmentCommandHandler(repo));

			IEventDispatcher evtDispatcher = new EventDispatcher();
			evtDispatcher.Register(new AppointmentEventHandler(readModelStorage));

			IMessageReceiver receiver = new InMemoryMessageReceiver();

			CommandProcessor commandProcessor = new CommandProcessor(receiver, serializer, cmdDispatcher); 
			EventPorcessor eventProcessor = new EventPorcessor(receiver, serializer, evtDispatcher);

			commandProcessor.Start();
			eventProcessor.Start();

			ICommandBus commandBus = new CommandBus(sender, serializer);

			//Test
			var command = new MakeAppointment();
			commandBus.Publish(command);

			//Verify
			Assert.AreEqual(1, 1);
		}
	}
}

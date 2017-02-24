using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading;
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
            var connStr = @"Server=192.168.88.79\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
			
            IMessageSender sender = new SqlMessageSender(connStr, "dbo.commands");
			ITextSerializer serializer = new JsonTextSerializer();
			IEventStore eventStore = new SqlEventStore(connStr, "dbo.events");

			IEventBus eventBus = new EventBus(sender, serializer);
			IMetadataProvider metaProvider = new StandardMetadataProvider();
			IReadModelStorage<AppointmentReadModel> readModelStorage = new InMemeoryStorage<AppointmentReadModel>();

			IEventSourcedRepository<AppointmentAggregate> repo = new
				EventSourcedRepository<AppointmentAggregate>(eventStore, eventBus, serializer, metaProvider);
			ICommandDispatcher cmdDispatcher = new CommandDispatcher();
			cmdDispatcher.Register(new AppointmentCommandHandler(repo));

			IEventDispatcher evtDispatcher = new EventDispatcher();
			evtDispatcher.Register(new AppointmentEventHandler(readModelStorage));

			IMessageReceiver cmdReceiver = new SqlMessageReceiver(connStr, "dbo.commands");

			IMessageReceiver evtReceiver = new SqlMessageReceiver(connStr, "dbo.events");

			CommandProcessor commandProcessor = new CommandProcessor(cmdReceiver, serializer, cmdDispatcher); 
			EventPorcessor eventProcessor = new EventPorcessor(evtReceiver, serializer, evtDispatcher);


			commandProcessor.Start();
			
			eventProcessor.Start();

			ICommandBus commandBus = new CommandBus(sender, serializer);

			//Test
			var command = new MakeAppointment();
			commandBus.Publish(command);
			Thread.Sleep(3000);
			
			var task = readModelStorage.GetAll(0, 10);
			task.Wait();

			Thread.Sleep(3000);
			
			//Verify
			Assert.AreEqual(1, task.Result.Count());
		}
	}
}

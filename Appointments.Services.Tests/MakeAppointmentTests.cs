﻿using System;
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
//            var connStr = @"Server=192.168.88.79\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
            var connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";

            IMessageSender sender = new SqlMessageSender(connStr, "dbo.messages");
			ITextSerializer serializer = new JsonTextSerializer();
//			IEventStore eventStore = new SqlEventStore(connStr, "dbo.events");
//
//			IEventBus eventBus = new EventBus(sender, serializer);
//			IMetadataProvider metaProvider = new StandardMetadataProvider();
			IReadModelStorage<AppointmentReadModel> readModelStorage = new InMemeoryStorage<AppointmentReadModel>();
//
//			IEventSourcedRepository<AppointmentAggregate> repo = new
//				EventSourcedRepository<AppointmentAggregate>(eventStore, eventBus, serializer, metaProvider);
//			ICommandDispatcher cmdDispatcher = new CommandDispatcher();
//			cmdDispatcher.Register(new AppointmentCommandHandler(repo));
//
//			IEventDispatcher evtDispatcher = new EventDispatcher();
//			evtDispatcher.Register(new AppointmentEventHandler(readModelStorage));
//
//			IMessageReceiver cmdReceiver = new SqlMessageReceiver(connStr, "dbo.commands");
//
//			IMessageReceiver evtReceiver = new SqlMessageReceiver(connStr, "dbo.events");
//
//			CommandProcessor commandProcessor = new CommandProcessor(cmdReceiver, serializer, cmdDispatcher); 
//			EventPorcessor eventProcessor = new EventPorcessor(evtReceiver, serializer, evtDispatcher);
//
//
//			commandProcessor.Start();
//			
//			eventProcessor.Start();

			ICommandBus commandBus = new CommandBus(sender, serializer);

			//Test
			var command = new MakeAppointment();
		    command.Appointment = new Appointment();
		    command.Appointment.Body = "Dental appointment";
		    command.Appointment.Subject = "Dental";
		    command.Appointment.Start = DateTimeOffset.MinValue;
		    command.Appointment.End = DateTimeOffset.MaxValue;
		    command.Appointment.Organizer = "Jeff Jin";
			var cmdTask = commandBus.Publish(new []{command});
		    cmdTask.Wait();

//			var task = readModelStorage.GetAll(0, 10);
//			task.Wait();
//
//			Thread.Sleep(3000);
//			
//			//Verify
//			Assert.AreEqual(1, task.Result.Count());
		}
	}
}

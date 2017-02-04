﻿using System;
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

			IMessageReceiver cmdReceiver = new InMemoryMessageReceiver();

			IMessageReceiver evtReceiver = new InMemoryMessageReceiver();

			CommandProcessor commandProcessor = new CommandProcessor(cmdReceiver, serializer, cmdDispatcher); 
			EventPorcessor eventProcessor = new EventPorcessor(evtReceiver, serializer, evtDispatcher);


			Task.Factory.StartNew(() =>
			{
				commandProcessor.Start();
			}, TaskCreationOptions.LongRunning);
			
			Task.Factory.StartNew(() =>
			{
				eventProcessor.Start();
			}, TaskCreationOptions.LongRunning);

			ICommandBus commandBus = new CommandBus(sender, serializer);

			//Test
			var command = new MakeAppointment();
			commandBus.Publish(command);
			Thread.Sleep(2000);
			
			var task = readModelStorage.GetAll(0, 10);
			task.Wait();

			Thread.Sleep(2000);
			
			//Verify
			Assert.AreEqual(1, task.Result.Count());
		}
	}
}

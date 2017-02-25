using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.Aggregates;
using Appointments.Commands;
using Appointments.EventHandlers;

namespace EventSource.TestProcessors
{
     public class CommandProcessorTest
    {
         public static void Run(string connStr)
         {

            IMessageSender sender = new SqlMessageSender(connStr, "dbo.commands");
            ITextSerializer serializer = new JsonTextSerializer();
            IEventStore eventStore = new SqlEventStore(connStr, "dbo.events");

            IEventBus eventBus = new EventBus(sender, serializer);
            IMetadataProvider metaProvider = new StandardMetadataProvider();

            IEventSourcedRepository<AppointmentAggregate> repo = new
                EventSourcedRepository<AppointmentAggregate>(eventStore, eventBus, serializer, metaProvider);
            ICommandDispatcher cmdDispatcher = new CommandDispatcher();
            cmdDispatcher.Register(new AppointmentCommandHandler(repo));

            IMessageReceiver cmdReceiver = new SqlMessageReceiver(connStr, "dbo.commands");

            CommandProcessor commandProcessor = new CommandProcessor(cmdReceiver, serializer, cmdDispatcher);
            commandProcessor.Start();
        }
    }
}

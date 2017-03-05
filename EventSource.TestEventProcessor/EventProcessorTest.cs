using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.Aggregates;
using Appointments.Dto;
using Appointments.EventHandlers;

namespace EventSource.TestProcessors
{
     public class EventProcessorTest
    {
         public static void Run(string connStr)
         {
            IMessageSender sender = new SqlMessageSender(connStr, "dbo.messages");
            ITextSerializer serializer = new JsonTextSerializer();

            IEventStore eventStore = new SqlEventStore(connStr, "dbo.events");
            IEventBus eventBus = new EventBus(sender, serializer);
            IMetadataProvider metaProvider = new StandardMetadataProvider();
            IReadModelStorage<AppointmentReadModel> readModelStorage = new InMemeoryStorage<AppointmentReadModel>();
            
            IEventSourcedRepository<AppointmentAggregate> repo = new
            	EventSourcedRepository<AppointmentAggregate>(eventStore, eventBus, serializer, metaProvider);
            
            IEventDispatcher evtDispatcher = new EventDispatcher();
            evtDispatcher.Register(new AppointmentEventHandler(readModelStorage));
            
            var evtReceiver = new SqlEventReceiver(new SqlEventStore(connStr, "dbo.events"));
            
            var eventProcessor = new EventProcessor(evtReceiver, serializer, evtDispatcher);
            			
            eventProcessor.Start();
        }
    }
}

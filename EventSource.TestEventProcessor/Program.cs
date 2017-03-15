using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.EventHandlers;
using EventSource.TestProcessors;

namespace EventSource.TestEventProcessor
{
    class Program
    {
        static string connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";

        static void Main(string[] args)
        {
            EventProcessorTest.Run(connStr);

            Console.ReadLine();
        }

        private static void StartEventReceiver()
        {
            var eventStore = new SqlEventStore(connStr, "dbo.events");
            var receiver = new SqlEventReceiver(eventStore);
            receiver.Start((EventData evt) =>
            {
                Console.WriteLine(evt.Payload);
            });

        }
    }
}
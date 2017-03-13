﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.EventHandlers;

namespace EventSource.TestProcessors
{
    class Program
    {
        static string connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
        static string table = "dbo.events";

        static void Main(string[] args)
        {
            StartMessageReceiver();

            Console.ReadLine();
        }

        private static void StartEventReceiver()
        {
            var eventStore = new SqlEventStore(connStr, table);
            var receiver = new SqlEventReceiver(eventStore);
            receiver.Start((EventData evt) =>
            {
                Console.WriteLine(evt.Payload);
            });

        }
        private static void StartMessageReceiver()
        {
            SqlMessageReceiverTest.RunReiver(connStr);
        }

        private static void StartCommandProcessor()
        {
            CommandProcessorTest.Run(connStr);
        }
    }
}
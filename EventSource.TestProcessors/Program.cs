using System;
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
            StartCommandProcessor();

            Console.ReadLine();
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
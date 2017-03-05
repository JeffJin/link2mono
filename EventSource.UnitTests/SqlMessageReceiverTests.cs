using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Appointments.EventHandlers;
using NUnit.Framework;

namespace EventSource.Tests
{
    [TestFixture]
    public class SqlEventReceiverTests
    {
        private string connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
        private string table = "dbo.events";
        [Test]
        public void TestReceiverMessage()
        {
            var eventStore = new SqlEventStore(connStr, table);
            var receiver = new SqlEventReceiver(eventStore);
            receiver.Start((EventData evt)=>
            {
                Console.WriteLine(evt.Payload);
            });


        }
    }
}

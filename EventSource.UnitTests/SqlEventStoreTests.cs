using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using Appointments.EventHandlers;
using NUnit.Framework;

namespace EventSource.Tests
{
    [TestFixture]
    public class SqlEventStoreTests
    {
        [Test]
        public void TestSaveEvents()
        {
            var connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
            var eventStore = new SqlEventStore(connStr, "dbo.events");
            var evt1 = new EventData()
            {
                SourceId = Guid.NewGuid(),
                Payload = "payload 1",
                AssemblyName = "ass 1",
                FullName = "full 1",
                CorrelationId = Guid.Empty,
                Namespace = "ns",
                SourceType = "st",
                TypeName = "tn",
                Version = 1
            };
//            var evt2 = new EventData();
//            var evt3 = new EventData();
            var task = eventStore.SaveEvents(new List<EventData> {evt1});
            task.Wait();
        }
    }
}

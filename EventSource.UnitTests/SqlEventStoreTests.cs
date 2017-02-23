using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Appointments.EventHandlers;
using NUnit.Framework;

namespace EventSource.Tests
{
    [TestFixture]
    public class SqlEventStoreTests
    {
        private string connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
        private string table = "dbo.events";

        [Test]
        public void TestSaveEvents()
        {
            var eventStore = new SqlEventStore(connStr, table);
            var correlationId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var evt1 = new EventData()
            {
                SourceId = sourceId,
                Payload = "payload 1",
                AssemblyName = "ass 1",
                FullName = "full 1",
                CorrelationId = correlationId,
                Namespace = "ns",
                SourceType = "st",
                TypeName = "tn",
                Version = 1
            };
            var evt2 = new EventData()
            {
                SourceId = sourceId,
                Payload = "payload 2",
                AssemblyName = "ass 2",
                FullName = "full 2",
                CorrelationId = correlationId,
                Namespace = "ns",
                SourceType = "st",
                TypeName = "tn",
                Version = 2
            };
            var evt3 = new EventData()
            {
                SourceId = sourceId,
                Payload = "payload 3",
                AssemblyName = "ass 3",
                FullName = "full 3",
                CorrelationId = correlationId,
                Namespace = "ns",
                SourceType = "st",
                TypeName = "tn",
                Version = 3
            };
//            var task = eventStore.SaveEvent(evt1);
            var task = eventStore.SaveEvents(new List<EventData> {evt1, evt2, evt3});
            task.Wait();

            var readTask = eventStore.LoadEvents(sourceId);
            readTask.Wait();
            Assert.AreEqual(readTask.Result.Count(), 3);

            eventStore.DeleteEvents(sourceId).Wait();
        }
    }
}

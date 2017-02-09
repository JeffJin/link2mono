using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;

namespace EventSource.Tests
{
    [TestClass]
    public class SqlMessageSenderTests
    {
        [TestMethod]
        public void TestSendMessage()
        {
            var connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
            var sender = new SqlMessageSender(connStr, "dbo.commands");
            sender.Send(new Message("message body", DateTime.Today, "correlation Id"));

        }
    }
}

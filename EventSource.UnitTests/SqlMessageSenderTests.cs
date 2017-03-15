using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using NUnit.Framework;

namespace EventSource.Tests
{
    [TestFixture]
    public class SqlMessageSenderTests
    {
        [Test]
        public void TestSendMessage()
        {
            var connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
            var sender = new SqlMessageSender(connStr, "dbo.messages");
            var msg1 = new Message("message body1", DateTime.Today, "12345678");
            var msg2 = new Message("message body2", DateTime.Today, "12345678");
            var msg3 = new Message("message body3", DateTime.Today, "12345678");
//            var task = sender.Send(msg1);
            var task = sender.Send(new List<Message>{msg1, msg2, msg3});
            task.Wait();
            var connectionFactory = new SqlConnectionFactory(connStr);
            var readQuery = string.Format(
                CultureInfo.InvariantCulture,
                @"SELECT TOP(1)
                    {0}.[Id] AS [Id], 
                    {0}.[Body] AS [Body], 
                    {0}.[DeliveryDate] AS [DeliveryDate],
                    {0}.[CorrelationId] AS [CorrelationId]
                    FROM {0} WITH (UPDLOCK, READPAST)
                    WHERE ({0}.[DeliveryDate] IS NULL) OR ({0}.[DeliveryDate] <= @CurrentDate)
                    ORDER BY {0}.[Id] ASC",
                "messages");
            using (var connection = connectionFactory.CreateConnection(connStr))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = readQuery;
                    ((SqlCommand) command).Parameters.Add("@CurrentDate", SqlDbType.DateTime).Value = DateTime.Now;

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return;
                        }

                        var body = (string) reader["Body"];
                        var deliveryDateValue = reader["DeliveryDate"];
                        var deliveryDate = deliveryDateValue == DBNull.Value
                            ? (DateTime?) null
                            : new DateTime?((DateTime) deliveryDateValue);
                        var correlationId = (string) reader["CorrelationId"];

                        Assert.IsTrue(body.Contains("message body"));
                        Assert.AreEqual(correlationId, "12345678");
                        Assert.AreEqual(deliveryDate.Value.Date, DateTime.Today.Date);
                    }
                }
            }

        }
    }
}

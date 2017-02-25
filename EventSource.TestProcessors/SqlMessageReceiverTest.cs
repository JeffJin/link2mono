using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.TestProcessors
{
    public class SqlMessageReceiverTest
    {
        public static void RunReiver(string connStr)
        {
            var receiver = new SqlMessageReceiver(connStr, "dbo.commands");
            receiver.Start((message) =>
            {
                Console.WriteLine(message.Body);
            });
        }
    }
}

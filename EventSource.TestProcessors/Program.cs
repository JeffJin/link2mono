using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.TestProcessors
{
    class Program
    {
        static void Main(string[] args)
        {
            var connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";
            var receiver = new SqlMessageReceiver(connStr, "dbo.commands");
            receiver.Start((message) =>
            {
                Console.WriteLine(message.Body);
            });

            Console.ReadLine();
        }
    }
}

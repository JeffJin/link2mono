using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource.TestProcessors
{
    class Program
    {
        static string connStr = @"Data Source=.\SQLEXPRESS;Database=appointments;User Id=chinook;Password=pr0t3ct3d";

        static void Main(string[] args)
        {
            SqlMessageReceiverTest.RunReiver(connStr);

            Console.ReadLine();
        }
    }
}

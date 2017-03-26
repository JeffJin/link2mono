using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appointments.Aggregates;
using Appointments.Commands;
using Appointments.Dto;
using Appointments.EventHandlers;
using Appointments.ReadModel;
using EventSource;
using NUnit.Framework;

namespace Appointments.Services.Tests
{
    [TestFixture]
	public class SqlReadModelContextTests
	{
	    [Test]
	    public void TestSaveAppointment()
	    {
	        using (var context = new ReadModelContext())
	        {
	            context.Appointments.Add(new AppointmentReadModel()
	            {
	                Start = DateTimeOffset.UtcNow,
	                End = DateTimeOffset.Now,
	                AttendeeNames = "Jeff, Mike",
	                Body = "appointment body",
	                Organizer = "Jeff",
	                Subject = "Test Subject",
	                TimeZoneOffset = 5
	            });
	            context.SaveChanges();
	        }

	        using (var context = new ReadModelContext())
	        {
	            var item = context.Appointments.FirstOrDefault();
	            Assert.AreEqual(item.Organizer, "Jeff");
	        }
	    }
	}
}

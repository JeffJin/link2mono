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
	public class SqlReadModelStorageTests
	{
	    [Test]
	    public void TestSaveAppointment()
	    {
	        var item = new AppointmentReadModel()
	        {
	            Start = DateTimeOffset.UtcNow,
	            End = DateTimeOffset.Now,
	            AttendeeNames = "Jeff, Mike",
	            Body = "appointment body",
	            Organizer = "Jeff",
	            Subject = "Test Subject",
	            TimeZoneOffset = 5
	        };
	        Func<ReadModelContext> contextFactory = () => new ReadModelContext();
            IReadModelStorage<AppointmentReadModel> repo = new SqlReadModelStorage(contextFactory);

	        repo.Save(item).Wait();

	        var task = repo.GetAll(0, 1);
	        task.Wait();

	        var model = task.Result.SingleOrDefault();
            Assert.AreEqual(model.Start, item.Start);
	    }
	}
}

using System;
using System.Collections.Generic;
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
	public class AppointmentEventHandlerTests
    {
	    [Test]
	    public void TestSaveAppointment()
	    {
	        var item = new Appointment()
	        {
	            Start = DateTimeOffset.UtcNow,
	            End = DateTimeOffset.Now,
	            Attendees = new List<Person>(){
                    new Person()
	                {
	                    Email = "jeff@jeffjin.com",
                        Name = "Jeff Jin"
	                },
                    new Person(){
	                    Email = "inyu@jeffjin.com",
                        Name = "Inyu Jin"
	                }
                },
	            Body = "appointment body",
	            Organizer = "Jeff",
	            Subject = "Test Subject"
	        };
            var evt = new AppointmentCreated(item);

	        Func<ReadModelContext> contextFactory = () => new ReadModelContext();
            IReadModelStorage<AppointmentReadModel> repo = new SqlReadModelStorage(contextFactory);

	        var handler = new AppointmentEventHandler(repo);
            handler.Handle(evt);

	        var task = repo.GetAll(0, 1);
	        task.Wait();

	        var model = task.Result.SingleOrDefault();
            Assert.AreEqual(model.Start, item.Start);
	    }
	}
}

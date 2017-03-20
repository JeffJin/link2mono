using System;
using System.Linq;
using Appointments.Aggregates;
using Appointments.Dto;
using EventSource;

namespace Appointments.EventHandlers
{
	public class AppointmentEventHandler: IEventHandler<AppointmentCreated>
	{
		readonly IReadModelStorage<AppointmentReadModel> storage;

		public AppointmentEventHandler(IReadModelStorage<AppointmentReadModel> storage)
		{
			this.storage = storage;
		}

		public void Handle(AppointmentCreated evt)
		{
			Console.WriteLine("AppointmentEventHandler Handle " + evt.ToString());
			
			AppointmentReadModel readModel = new AppointmentReadModel();
		    readModel.Id = evt.Appointment.Id;
            readModel.Body = evt.Appointment.Body;
			readModel.Subject = evt.Appointment.Subject;
			readModel.Start = evt.Appointment.Start;
			readModel.End = evt.Appointment.End;
			readModel.Organizer = evt.Appointment.Organizer;
			readModel.TimeZoneOffset = evt.Appointment.TimeZoneOffset;
			readModel.AttendeeNames = String.Concat(", ", evt.Appointment.Attendees.Select(a => a.Name));

			this.storage.Save(readModel);
		}
	}
}

using System;
using System.Collections.Generic;
using EventSource;

namespace Appointments.Dto
{
	public class AppointmentReadModel: IReadModel
	{
		public AppointmentReadModel()
        {
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }

		public DateTimeOffset Start { get; set; }

		public DateTimeOffset End { get; set; }

		public TimeSpan TimeZoneOffset { get; }

		public string Subject { get; set; }

		public string Body { get; set; }
	
		//email address of the organizer
		public string Organizer { get; }

		public string AttendeeNames { get; }


		
	}

	
}

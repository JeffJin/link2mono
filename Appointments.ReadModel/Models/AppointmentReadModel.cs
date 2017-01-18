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

		public int TimeZoneOffset { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
	
		//email address of the organizer
		public string Organizer { get; set;}

		public string AttendeeNames { get; set;}


		
	}

	
}

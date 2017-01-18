using System;
using System.Collections.Generic;

namespace Appointments.Dto
{
	public class Appointment
	{
		public Appointment()
        {
			Id = Guid.NewGuid();
		}

		public Guid Id { get; }

		public DateTimeOffset Start { get; set; }

		public DateTimeOffset End { get; set; }

		public int TimeZoneOffset { get; }

		public string Subject { get; set; }

		public string Body { get; set; }
	
		//email address of the organizer
		public string Organizer { get; }

		public IList<Person> Attendees { get; }


		
	}

	
}

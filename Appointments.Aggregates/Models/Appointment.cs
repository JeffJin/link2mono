using System;
using System.Collections.Generic;

namespace Appointments.Aggregates
{
	public class Appointment
	{
		public Appointment()
		{
		}

		public DateTimeOffset Start { get; set; }

		public DateTimeOffset End { get; set; }

		public TimeSpan TimeZoneOffset { get; }

		public string Subject { get; set; }

		public string Body { get; set; }
	
		//email address of the organizer
		public string Organizer { get; }

		public IList<Person> Attendees { get; }
		
	}

	
}

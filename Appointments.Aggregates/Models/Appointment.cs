using System;
using System.Collections.Generic;
using EventSource;

namespace Appointments.Aggregates
{
	public class Appointment: EventSourced
	{
		public Appointment(Guid id) : base(id)
        {
			Init();
		}

		public Appointment(Guid id, IEnumerable<IVersionedEvent> history): this(id)
		{
			this.LoadFrom(history);
			//Update(new AppointmentCreated(info));
		}

		private void Init()
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

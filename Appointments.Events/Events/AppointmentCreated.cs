using Appointments.Dto;
using EventSource;

namespace Appointments.Aggregates
{
	public class AppointmentCreated : VersionedEvent
	{
		private Appointment Appointment { get; }

		public AppointmentCreated(Appointment appt)
		{
			this.Appointment = appt;
		}
	}
}
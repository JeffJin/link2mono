using EventSource;

namespace Appointments.Aggregates
{
	public class AppointmentCreated : VersionedEvent
	{
		object info;

		public AppointmentCreated(object info)
		{
			this.info = info;
		}
	}
}
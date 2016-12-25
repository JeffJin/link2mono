namespace Appointments.Aggregates
{
	public class AppointmentCreated
	{
		Appointment appointment;

		public AppointmentCreated(Appointment appointment)
		{
			this.appointment = appointment;
		}
	}
}
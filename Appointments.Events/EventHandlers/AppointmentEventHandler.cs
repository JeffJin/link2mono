using System;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.Events
{
	public class AppointmentEventHandler: IEventHandler<AppointmentCreated>
	{
		readonly IEventPersistence repository;

		public AppointmentEventHandler(IEventPersistence repository)
		{
			this.repository = repository;
		}

		public void Handle(AppointmentCreated @event)
		{
			throw new NotImplementedException();
		}
	}
}

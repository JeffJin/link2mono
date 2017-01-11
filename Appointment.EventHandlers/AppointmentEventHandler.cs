using System;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.EventHandlers
{
	public class AppointmentEventHandler: IEventHandler<AppointmentCreated>
	{
		readonly IReadModelRepository repository;

		public AppointmentEventHandler(IReadModelRepository repository)
		{
			this.repository = repository;
		}

		public void Handle(AppointmentCreated @event)
		{
			throw new NotImplementedException();
		}
	}
}

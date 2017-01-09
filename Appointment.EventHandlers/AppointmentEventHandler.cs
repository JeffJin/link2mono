using System;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.EventHandlers
{
	public class AppointmentEventHandler: IEventHandler<AppointmentCreated>
	{
		readonly IEventSourcedRepository<AppointmentAggregate> repository;

		public AppointmentEventHandler(IEventSourcedRepository<AppointmentAggregate> repository)
		{
			this.repository = repository;
		}

		public void Handle(AppointmentCreated @event)
		{
			throw new NotImplementedException();
		}
	}
}

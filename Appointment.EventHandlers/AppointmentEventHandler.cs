using System;
using Appointments.Aggregates;
using Appointments.Dto;
using Appointments.EventHandlers;
using EventSource;

namespace Appointments.EventHandlers
{
	public class AppointmentEventHandler: IEventHandler<AppointmentCreated>
	{
		readonly IReadModelStoragte<AppointmentReadModel> storage;

		public AppointmentEventHandler(IReadModelStoragte<AppointmentReadModel> storage)
		{
			this.storage = storage;
		}

		public void Handle(AppointmentCreated @event)
		{

		}
	}
}

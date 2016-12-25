using System;
using System.Collections.Generic;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.Commands
{
	public class AppointmentCommandHandler: ICommandHandler<MakeAppointment>
	{
		private readonly IEventSourcedRepository<AppointmentAggregate> _repository;

		public AppointmentCommandHandler(IEventSourcedRepository<AppointmentAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(MakeAppointment command)
		{
			AppointmentAggregate appointmentAggregte = _repository.Find(command.Id);
			if (appointmentAggregte == null)
			{
				throw new AggregateNotFoundException(command.Id, "Appointment");
			}

			appointmentAggregte.AddNewAppointment(appointmentAggregte, command.Appointment);
			_repository.Save(appointmentAggregte, command.Id.ToString());
		}
	}


}

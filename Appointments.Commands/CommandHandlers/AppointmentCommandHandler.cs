using System;
using System.Collections.Generic;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.Commands
{
	public class AppointmentCommandHandler: ICommandHandler<MakeAppointment>
	{
		private readonly IEventSourcedRepository<Appointment> _repository;

		public AppointmentCommandHandler(IEventSourcedRepository<Appointment> repository)
		{
			_repository = repository;
		}

		public void Handle(MakeAppointment command)
		{
			throw new NotImplementedException();
		}
	}


}

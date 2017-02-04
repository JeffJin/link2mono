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
			Console.WriteLine("AppointmentCommandHandler Handle " + command.ToString());
			
			_repository.Find(command.Id).ContinueWith((task) =>
			{
				var appointmentAggregate = task.Result;
				if (appointmentAggregate != null)
				{
					throw new DuplicatedAggregateException(command.Id, "Appointment");
				}
				else
				{
					appointmentAggregate = new AppointmentAggregate(command.Id);
				}

				appointmentAggregate.CreateAppointment(command.Appointment);
				var saveTask = _repository.Save(appointmentAggregate, command.Id);
				saveTask.Wait();
			});
		}
	}


}

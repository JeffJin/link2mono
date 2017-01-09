using System;
using System.Collections.Generic;
using EventSource;
using Appointments.Dto;


namespace Appointments.Aggregates
{
	public class AppointmentAggregate: EventSourced
	{
		
		public AppointmentAggregate(Guid id) : base(id)
        {
		}

		//this is requirement for EventStoreRepository 
		//var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
		public AppointmentAggregate(Guid id, IEnumerable<IVersionedEvent> history) : this(id)
		{
			this.LoadFrom(history);
		}
	
		public void CreateAppointment(Appointment info)
		{
			var evt = new AppointmentCreated(info);

			this.Update(evt);
		}
	}

	
}

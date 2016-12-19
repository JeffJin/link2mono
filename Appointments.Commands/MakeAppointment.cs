﻿using System;
using Appointments.Aggregates;
using EventSource;

namespace Appointments.Commands
{
	public class MakeAppointment : ICommand
	{
		public MakeAppointment()
		{
		}

		public Guid Id { get; set; }

		public Appointment Appointment{ get; set; }
	}
}

using System;
using System.Collections.Generic;

namespace EventSource
{

	public abstract class VersionedEvent : IVersionedEvent
	{
		public Guid SourceId { get; set; }

		public int Version { get; set; }
	}
}

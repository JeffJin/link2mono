using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventData// : IEvent
	{
        //Aggregate ID
		public Guid SourceId { get; set; }
		public int Version { get; set; }
        //Aggregate Type
		public string SourceType { get; set; }
		public string Payload { get; set; }
		public Guid CorrelationId { get; set; }

		// Standard metadata.
		public string AssemblyName { get; set; }
		public string Namespace { get; set; }
		public string FullName { get; set; }
		public string TypeName { get; set; }
	}
//
//    public class Event
//    {
//        public Guid AggregateId { get; set; }
//        public string AggregateType { get; set; }
//        public int Version { get; set; }
//        public string Payload { get; set; }
//        public string CorrelationId { get; set; }
//
//        // TODO: Following could be very useful for when rebuilding the read model from the event store, 
//        // to avoid replaying every possible event in the system
//        // public string EventType { get; set; }
//    }

}

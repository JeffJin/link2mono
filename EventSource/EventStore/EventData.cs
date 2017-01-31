using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public class EventData
	{
		public string SourceId { get; set; }
		public int Version { get; set; }
		public string SourceType { get; set; }
		public string Payload { get; set; }
		public string CorrelationId { get; set; }

		// Standard metadata.
		public string AssemblyName { get; set; }
		public string Namespace { get; set; }
		public string FullName { get; set; }
		public string TypeName { get; set; }
	}
	
}

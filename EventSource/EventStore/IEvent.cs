using System;

namespace EventSource
{

	/// <summary>
	/// Represents an event message.
	/// </summary>
	public interface IEvent
	{
		/// <summary>
		/// Gets the identifier of the source originating the event.
		/// </summary>
		Guid SourceId { get; set;}
		Guid? CorrelationId { get; set;}
		string Payload { get; set; }
	}

	
}

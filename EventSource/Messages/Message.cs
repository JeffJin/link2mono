using System;

namespace EventSource
{
	public class Message
	{
		public Message(string body, DateTime? deliveryDate = null, string correlationId = null)
		{
		    this.Id = Guid.NewGuid();
			this.Body = body;
			this.DeliveryDate = deliveryDate;
			this.CorrelationId = correlationId;
		}

	    public Guid Id { get; set; }

	    public string Body { get; private set; }

		public string CorrelationId { get; private set; }

		public DateTime? DeliveryDate { get; private set; }
	}
}
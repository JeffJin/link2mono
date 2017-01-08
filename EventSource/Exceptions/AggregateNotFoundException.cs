using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace EventSource
{

    [Serializable]
	public class AggregateNotFoundException : Exception
	{
		private readonly Guid entityId;
		private readonly string entityType;

		public AggregateNotFoundException()
		{
		}

		public AggregateNotFoundException(Guid entityId) : base(entityId.ToString())
		{
			this.entityId = entityId;
		}

		public AggregateNotFoundException(Guid entityId, string entityType)
			: base(entityType + ": " + entityId)
		{
			this.entityId = entityId;
			this.entityType = entityType;
		}

		public AggregateNotFoundException(Guid entityId, string entityType, string message, Exception inner)
			: base(message, inner)
		{
			this.entityId = entityId;
			this.entityType = entityType;
		}

		protected AggregateNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
			if (info == null)
				throw new ArgumentNullException(nameof(info));

			entityId = Guid.Parse(info.GetString("entityId"));
			entityType = info.GetString("entityType");
		}

		public Guid EntityId
		{
			get { return entityId; }
		}

		public string EntityType
		{
			get { return entityType; }
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("entityId", entityId.ToString());
			info.AddValue("entityType", entityType);
		}
	}
}

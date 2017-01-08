using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace EventSource
{

    [Serializable]
	public class DuplicatedAggregateException : Exception
	{
		private readonly Guid entityId;
		private readonly string entityType;

		public DuplicatedAggregateException()
		{
		}

		public DuplicatedAggregateException(Guid entityId) : base(entityId.ToString())
		{
			this.entityId = entityId;
		}

		public DuplicatedAggregateException(Guid entityId, string entityType)
			: base(entityType + ": " + entityId)
		{
			this.entityId = entityId;
			this.entityType = entityType;
		}

		public DuplicatedAggregateException(Guid entityId, string entityType, string message, Exception inner)
			: base(message, inner)
		{
			this.entityId = entityId;
			this.entityType = entityType;
		}

		protected DuplicatedAggregateException(
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

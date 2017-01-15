using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EventSource
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public MessageReceivedEventArgs(Message message)
		{
			this.Message = message;
		}

		public Message Message { get; private set; }
	}
}

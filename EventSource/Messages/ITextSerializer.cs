using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventSource
{

	public interface ITextSerializer
	{
		/// <summary>
		/// Serializes an object graph to a text reader.
		/// </summary>
		void 
		Serialize(TextWriter writer, object graph);

		/// <summary>
		/// Deserializes an object graph from the specified text reader.
		/// </summary>
		object Deserialize(TextReader reader);
	}
	
}

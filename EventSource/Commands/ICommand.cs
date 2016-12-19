using System;
namespace EventSource
{
	public interface ICommand
	{
		/// <summary>
		/// Gets the command identifier.
		/// </summary>
		Guid Id { get; }
	}
}

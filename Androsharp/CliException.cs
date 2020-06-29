#nullable enable
using System;

namespace Androsharp
{
	public sealed class CliException : Exception
	{
		public CliException() { }
		
		public CliException(string? message) : base(message) { }
	}
}
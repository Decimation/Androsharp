#nullable enable
using System;

namespace Androsharp.Model
{
	internal sealed class CliException : Exception
	{
		internal CliException() { }

		internal CliException(string? message) : base(message) { }
		
	}
}
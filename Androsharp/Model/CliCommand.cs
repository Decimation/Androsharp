using System;
using System.Text;
using JetBrains.Annotations;
using static Androsharp.Utilities.Common;

namespace Androsharp.Model
{
	// todo

	public sealed class CliCommand
	{
		public string CommandStub { get; private set; }

		public CliScope CliScope { get; private set; }

		private CliCommand(string commandStub, CliScope s)
		{
			CommandStub = commandStub;
			CliScope = s;
		}

		
		[StringFormatMethod(FMT_ARG)]
		public static CliCommand Create(string fmt, params object[] args)
		{
			return Create(CliScope.None, fmt, args);
		}
		
		[StringFormatMethod(FMT_ARG)]
		public static CliCommand Create(CliScope s,string fmt, params object[] args)
		{
			var cmd  = string.Format(fmt, args);
			var ccmd = new CliCommand(cmd,s);


			return ccmd;
		}


		/*public static implicit operator CliCommand(string c)
		{
			return new CliCommand(c);
		}*/

		public string GetFullCommand()
		{
			const string SCOPE_ADB        = "adb ";
			const string SCOPE_ADBSHELL   = "adb shell ";
			const string SCOPE_ADBEXECOUT = "adb exec-out ";

			switch (CliScope) {
				case CliScope.None:
					return CommandStub;
				case CliScope.Adb:
					return SCOPE_ADB + CommandStub;
				case CliScope.AdbShell:
					return SCOPE_ADBSHELL + CommandStub;
				case CliScope.AdbExecOut:
					return SCOPE_ADBEXECOUT + CommandStub;
				default:
					throw new Exception();
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			//sb.AppendFormat("{0} (scope: {1})", FullCommand, Scope);
			sb.AppendFormat("Command: {0}", GetFullCommand());
			return sb.ToString();
		}
	}
}
using System;
using System.Text;
using JetBrains.Annotations;
using static Androsharp.Constants;

namespace Androsharp
{
	// todo

	public class CliCommand
	{
		public string Command { get; internal set; }

		public Scope Scope { get; internal set; }

		private CliCommand(string command)
		{
			Command = command;
			Scope   = Scope.None;
		}

		[StringFormatMethod(FMT_ARG)]
		public static CliCommand From(string fmt, params object[] args)
		{
			var cmd  = string.Format(fmt, args);
			var ccmd = new CliCommand(cmd);


			return ccmd;
		}

		public CliCommand WithScope(Scope s)
		{
			Scope = s;

			return this;
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

			switch (Scope) {
				case Scope.None:
					return Command;
					break;
				case Scope.Adb:
					return SCOPE_ADB + Command;
					break;
				case Scope.AdbShell:
					return SCOPE_ADBSHELL + Command;
					break;
				case Scope.AdbExecOut:
					return SCOPE_ADBEXECOUT + Command;
					break;
				default:
					throw new Exception();
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			//sb.AppendFormat("{0} (scope: {1})", FullCommand, Scope);
			sb.AppendFormat(">> {0}", GetFullCommand());
			return sb.ToString();
		}
	}
}
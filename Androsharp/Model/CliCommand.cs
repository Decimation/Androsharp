using System;
using System.Text;
using JetBrains.Annotations;
using static Androsharp.Model.Constants;

namespace Androsharp.Model
{
	// todo

	public class CliCommand
	{
		public string CommandStub { get; private set; }

		public Scope Scope { get; private set; }

		private CliCommand(string commandStub, Scope s)
		{
			CommandStub = commandStub;
			Scope = s;
		}

		
		[StringFormatMethod(FMT_ARG)]
		public static CliCommand Create(string fmt, params object[] args)
		{
			return Create(Scope.None, fmt, args);
		}
		
		[StringFormatMethod(FMT_ARG)]
		public static CliCommand Create(Scope s,string fmt, params object[] args)
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

			switch (Scope) {
				case Scope.None:
					return CommandStub;
					break;
				case Scope.Adb:
					return SCOPE_ADB + CommandStub;
					break;
				case Scope.AdbShell:
					return SCOPE_ADBSHELL + CommandStub;
					break;
				case Scope.AdbExecOut:
					return SCOPE_ADBEXECOUT + CommandStub;
					break;
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
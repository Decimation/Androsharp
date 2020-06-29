using System;
using System.Text;

namespace Androsharp
{
	// todo
	
	public class CliCommand
	{
		public string Command { get; }
		
		public string FullCommand { get; }
		
		public Scope Scope { get; }

		public CliCommand(string command, Scope scope = Scope.None)
		{
			Command = command;
			Scope = scope;
			FullCommand = CreateFullCommand(command);
		}

		public static implicit operator CliCommand(string c)
		{
			return new CliCommand(c);
		}

		private static string CreateFullCommand(string cmd, Scope scope = Scope.None)
		{
			const string SCOPE_ADB        = "adb ";
			const string SCOPE_ADBSHELL   = "adb shell ";
			const string SCOPE_ADBEXECOUT = "adb exec-out ";
			
			switch (scope) {
				case Scope.None:
					return cmd;
					break;
				case Scope.Adb:
					return SCOPE_ADB + cmd;
					break;
				case Scope.AdbShell:
					return SCOPE_ADBSHELL + cmd;
					break;
				case Scope.AdbExecOut:
					return SCOPE_ADBEXECOUT + cmd;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			//sb.AppendFormat("{0} (scope: {1})", FullCommand, Scope);
			sb.AppendFormat(">> {0}", FullCommand);
			return sb.ToString();
		}
	}
}
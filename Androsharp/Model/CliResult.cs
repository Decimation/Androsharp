#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Androsharp.Utilities;

namespace Androsharp.Model
{
	// todo

	public class CliResult
	{
		private CliResult(CliCommand command)
		{
			Command        = command;
			CommandProcess = null;
			Data           = null;
		}

		public CliCommand Command { get; }

		public object? Data { get; private set; }

		public DataType DataType { get; private set; }
		
		public Process? CommandProcess { get; private set; }

		public static CliResult Run(CliCommand command, DataType dt = DataType.None)
		{
			var cliResult = new CliResult(command)
			{
				DataType = dt
			};

			var proc = Cli.Shell(command.GetFullCommand());
			cliResult.CommandProcess = proc;


			proc.Start();

			var stdOut = proc.StandardOutput;

			switch (dt) {
				case DataType.None:
					break;
				case DataType.StringArray:
					cliResult.Data = CliUtilities.ReadAllLines(stdOut);
					break;
				case DataType.ByteArray:
					cliResult.Data = CliUtilities.ReadToEnd(stdOut.BaseStream);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
			}


			return cliResult;
		}

		public bool GetBinary(out byte[]? rg)
		{
			if (DataType == DataType.ByteArray) {
				rg = Data as byte[];
				return true;
			}

			rg = null;
			return false;
		}
		
		public bool GetLines(out string[]? rg)
		{
			if (DataType == DataType.StringArray) {
				rg = Data as string[];
				return true;
			}

			rg = null;
			return false;
		}

		public bool SuccessfulIfLineContains(string s)
		{
			if (DataType == DataType.StringArray) {
				var rg = Data as string[];
				return rg.Any(ln => ln.Contains(s));
			}


			return false;
		}
	}
}
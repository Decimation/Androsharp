#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Androsharp.Utilities;

namespace Androsharp.Model
{
	// todo

	public sealed class CliResult
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

		// todo: performance: ~1100ms
		
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
					// todo: performance ~900ms
					var end = CliUtilities.ReadToEnd(stdOut.BaseStream);
					lock (end) {
						cliResult.Data = end;
					}
					
					break;
				case DataType.Unknown:
					break;
				case DataType.String:
					cliResult.Data = stdOut.ReadToEnd();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
			}

			//!!!
			proc.Close();

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
		public bool GetStr(out string? s)
		{
			if (DataType == DataType.String) {
				s = Data as string;
				return true;
			}

			s = null;
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
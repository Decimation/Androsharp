#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Androsharp
{
	// todo
	
	public class CliResult
	{
		public CliCommand Command { get;  }
		
		public object? Data { get; private set; }

		public bool? IsSuccessful { get; private set; }
		
		public DataType DataType { get; private set; }
		

		private CliResult(CliCommand command)
		{
			Command = command;
			
			Data = null;
			IsSuccessful = null;
		}
		

		public static CliResult Run(CliCommand command, DataType dt = DataType.Unknown)
		{
			var cliResult = new CliResult(command)
			{
				DataType = dt
			};

			var proc = Cli.Shell(command.GetFullCommand());
			proc.Start();

			var stdOut = proc.StandardOutput;

			switch (dt) {
				case DataType.Unknown:
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
		

		public CliResult SuccessfulIfLineContains(string s)
		{
			if (DataType == DataType.StringArray) {
				var rg = Data as string[];
				IsSuccessful = rg.Any(ln => ln.Contains(s));
			}
			
			

			return this;
		}

		
		
	}
}
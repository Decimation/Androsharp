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

		public CliDataType DataType { get; private set; }

		public Process? CommandProcess { get; private set; }

		public byte[] ByteArray {
			get {
				return Parse<byte[]>(CliDataType.ByteArray);
			}
		}

		public string[] StringArray {
			get {
				return Parse<string[]>(CliDataType.StringArray);
			}
		}

		public string String {
			get {
				return Parse<string>(CliDataType.String);
			}
		}
		
		internal T Parse<T>(CliDataType dt)
		{
			if (DataType != dt) {
				return default;
			}

			return (T)  ((object) Data);
		}

		// todo: performance: ~1100ms

		public static CliResult Run(CliCommand command, CliDataType dt = CliDataType.None)
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
				case CliDataType.None:
					break;
				case CliDataType.StringArray:
					cliResult.Data = CliUtilities.ReadAllLines(stdOut);
					break;
				case CliDataType.ByteArray:
					// todo: performance ~900ms !!!
					var end = CliUtilities.ReadToEnd(stdOut.BaseStream);
					lock (end) {
						cliResult.Data = end;
					}

					break;
				case CliDataType.String:
					cliResult.Data = stdOut.ReadToEnd();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
			}

			//!!!
			proc.Close();

			return cliResult;
		}

		public bool SuccessfulIfLineContains(string s)
		{
			if (DataType == CliDataType.StringArray) {
				var rg = Data as string[];
				return rg.Any(ln => ln.Contains(s));
			}


			return false;
		}
	}
}
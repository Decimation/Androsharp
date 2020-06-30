#nullable enable
using System;
using System.Diagnostics;
using Androsharp.Model;
using JetBrains.Annotations;

namespace Androsharp.Utilities
{
	// todo

	public sealed class Android
	{
		public static readonly string BUSYBOX_EXE = @"C:\Users\Deci\Desktop\busybox";

		public static readonly string BUSYBOX_VER = "BusyBox v1.31.1-meefik (2020-03-11 18:59:07 UTC)";

		public static readonly string REMOTE_BUSYBOX_EXE = "/data/local/tmp/busybox";

		private Android()
		{
			Serial = GetSerial();
		}


		/// <summary>
		/// Device serial
		/// </summary>
		public string? Serial { get; }

		public static Android Value { get; private set; } = new Android();


		public bool HasBusybox {
			get {
				//
				// Check busybox status
				//


				CliCommand bbCmd    = CliCommand.Create(Scope.AdbExecOut, REMOTE_BUSYBOX_EXE);
				var        bbResult = CliResult.Run(bbCmd, DataType.StringArray);
				var        bbOk     = bbResult.SuccessfulIfLineContains(BUSYBOX_VER);


				return bbOk;
			}
		}

		public long RemoteSize(string s)
		{
			// sprintf(buf, "wc -c < \"%s\"", remoteFile);
			
			var cmd = CliCommand.Create(Scope.AdbShell, "\"wc -c < \"{0}\"\"", s);
			var res = CliResult.Run(cmd, DataType.String);

			//Console.WriteLine(res.Data);
			var n = long.Parse((string) res.Data);


			return n;
		}
		public string ReadFile(string s)
		{
			var cmd = CliCommand.Create(Scope.AdbShell, "cat {0}", s);
			var res = CliResult.Run(cmd, DataType.String);


			res.GetStr(out var resStr);

			return resStr;
		}

		private static string? GetSerial()
		{
			var devices       = CliCommand.Create(Scope.Adb, "devices");
			var devicesResult = CliResult.Run(devices, DataType.StringArray);

			if (!devicesResult.GetLines(out var resultLines)) {
				return null;
			}


			var hasDevice = resultLines?.Length >= 2 && resultLines[1] != null;

			Debug.Assert(resultLines != null);

			var device = hasDevice ? resultLines[1].SubstringBefore("device").Trim() : null;

			return device;
		}

		public void Reset()
		{
			// adb shell "rm /data/local/tmp/busybox"

			CliCommand rmCmd = CliCommand.Create(Scope.AdbShell, "rm {0}", REMOTE_BUSYBOX_EXE);

			var rmResult = CliResult.Run(rmCmd);
		}

		public void Setup()
		{
			//
			// adb push busybox /data/local/tmp/busybox
			//

			var pushCmd = CliCommand.Create(Scope.Adb, "push {0} {1}", BUSYBOX_EXE, REMOTE_BUSYBOX_EXE);

			var pushResult = CliResult.Run(pushCmd, DataType.StringArray);

			bool pushOk = pushResult.SuccessfulIfLineContains("busybox: 1 file pushed");


			if (!pushOk) {
				throw new CliException("Could not push busybox to device");
			}

			//
			// chmod +x /data/local/tmp/busybox
			//

			var chmodCmd = CliCommand.Create(Scope.AdbShell, "chmod +x {0}", REMOTE_BUSYBOX_EXE);

			var chmodResult = CliResult.Run(chmodCmd, DataType.StringArray);


			if (!HasBusybox) {
				throw new CliException("Busybox failure to device");
			}
		}

		public override string ToString()
		{
			return string.Format("Serial: {0}", Serial);
		}
	}
}
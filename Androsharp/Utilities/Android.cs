#nullable enable
using System;
using System.Diagnostics;
using Androsharp.Model;
using JetBrains.Annotations;

namespace Androsharp.Utilities
{
	// todo

	public static class Android
	{
		public static readonly string BUSYBOX_EXE = @"C:\Users\Deci\Desktop\busybox";

		public static readonly string BUSYBOX_VER = "BusyBox v1.31.1-meefik (2020-03-11 18:59:07 UTC)";

		public static readonly string REMOTE_BUSYBOX_EXE = "/data/local/tmp/busybox";


		/// <summary>
		/// Device serial
		/// </summary>
		public static string? Serial { get; } = GetSerial();

		public static bool HasDevice => Serial != null;
		
		public static bool HasBusybox {
			get {
				//
				// Check busybox status
				//
				
				CliCommand bbCmd    = CliCommand.Create(CliScope.AdbExecOut, REMOTE_BUSYBOX_EXE);
				var        bbResult = CliResult.Run(bbCmd, CliDataType.StringArray);
				var        bbOk     = bbResult.SuccessfulIfLineContains(BUSYBOX_VER);


				return bbOk;
			}
		}

		public static long GetFileSize(string s)
		{
			// sprintf(buf, "wc -c < \"%s\"", remoteFile);

			var cmd = CliCommand.Create(CliScope.AdbShell, "\"wc -c < \"{0}\"\"", s);
			var res = CliResult.Run(cmd, CliDataType.String);

			//Console.WriteLine(res.Data);
			var n = long.Parse((string) res.Data);


			return n;
		}

		public static string ReadFile(string remote)
		{
			var cmd = CliCommand.Create(CliScope.AdbShell, "cat {0}", remote);
			var res = CliResult.Run(cmd, CliDataType.String);

			return res.String;
		}

		public static string GetFileSha1Hash(string remote)
		{
			var cmd = CliCommand.Create(CliScope.AdbShell, "sha1sum \"{0}\"", remote);
			var res = CliResult.Run(cmd, CliDataType.String);
			var hash = res.String.Split(" ")[0];
			
			return hash.ToUpper();
		}

		private static string? GetSerial()
		{
			var devices       = CliCommand.Create(CliScope.Adb, "devices");
			var devicesResult = CliResult.Run(devices, CliDataType.StringArray);

			var resultLines = devicesResult.StringArray;


			var hasDevice = resultLines?.Length >= 2 && resultLines[1] != null;

			Debug.Assert(resultLines != null);

			var device = hasDevice ? resultLines[1].SubstringBefore("device").Trim() : null;

			return device;
		}

		public static void Reset()
		{
			// adb shell "rm /data/local/tmp/busybox"

			CliCommand rmCmd = CliCommand.Create(CliScope.AdbShell, "rm {0}", REMOTE_BUSYBOX_EXE);

			var rmResult = CliResult.Run(rmCmd);
		}

		public static void Setup()
		{
			if (!HasDevice) {
				throw new CliException("No device");
			}
			
			//
			// adb push busybox /data/local/tmp/busybox
			//

			var pushCmd = CliCommand.Create(CliScope.Adb, "push {0} {1}", BUSYBOX_EXE, REMOTE_BUSYBOX_EXE);

			var pushResult = CliResult.Run(pushCmd, CliDataType.StringArray);

			bool pushOk = pushResult.SuccessfulIfLineContains("busybox: 1 file pushed");


			if (!pushOk) {
				throw new CliException("Could not push busybox to device");
			}

			//
			// chmod +x /data/local/tmp/busybox
			//

			var chmodCmd = CliCommand.Create(CliScope.AdbShell, "chmod +x {0}", REMOTE_BUSYBOX_EXE);

			var chmodResult = CliResult.Run(chmodCmd, CliDataType.StringArray);


			if (!HasBusybox) {
				throw new CliException("Busybox failure to device");
			}
		}
	}
}
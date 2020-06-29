using System;
using System.Linq;

namespace Androsharp
{
	// todo

	public static class Android
	{
		public static readonly string BUSYBOX_EXE = @"C:\Users\Deci\Desktop\busybox";


		public static readonly string REMOTE_BUSYBOX_EXE = "/data/local/tmp/busybox";


		public static void Reset()
		{
			// adb shell "rm /data/local/tmp/busybox"

			CliCommand rmCmd = CliCommand.From("rm {0}", REMOTE_BUSYBOX_EXE)
			                             .WithScope(Scope.AdbShell);

			var rmResult = CliResult.Run(rmCmd);
		}

		public static void Init()
		{
			// adb push busybox /data/local/tmp/busybox

			CliCommand pushCmd = CliCommand.From("push {0} {1}", BUSYBOX_EXE, REMOTE_BUSYBOX_EXE)
			                               .WithScope(Scope.Adb);


			CliResult pushResult = CliResult.Run(pushCmd, DataType.StringArray)
			                                .SuccessfulIfLineContains("busybox: 1 file pushed");


			if (pushResult.IsSuccessful != null && !pushResult.IsSuccessful.Value) {
				throw new CliException("Could not push busybox to device");
			}

			// chmod +x /data/local/tmp/busybox

			CliCommand chmodCmd = CliCommand.From("chmod +x {0}", REMOTE_BUSYBOX_EXE);

			CliResult chmodResult = CliResult.Run(chmodCmd, DataType.StringArray)
			                                 .SuccessfulIfLineContains(
				                                  "BusyBox v1.31.1-meefik (2020-03-11 18:59:07 UTC) multi-call binary.");

			if (pushResult.IsSuccessful != null && !pushResult.IsSuccessful.Value) {
				throw new CliException("Busybox failure to device");
			}
		}
	}
}
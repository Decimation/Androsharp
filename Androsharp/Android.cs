using System.Linq;

namespace Androsharp
{
	public static class Android
	{
		public static readonly string BUSYBOX_EXE = @"C:\Users\Deci\Desktop\busybox";
		public static readonly string REMOTE_BUSYBOX = "/data/local/tmp/busybox";
		
		public static void Init()
		{
			// adb push busybox /data/local/tmp/busybox

			var pushCmd = string.Format("push {0} {1}", BUSYBOX_EXE, REMOTE_BUSYBOX);
			var push = Cli.Shell(pushCmd, false, Scope.Adb);
			var pushStdOut = CliUtilities.ReadAllLines(push.StandardOutput);
			var pushSuccess = pushStdOut.Any(ln => ln.Contains("busybox: 1 file pushed"));

			if (!pushSuccess) {
				throw new CliException("Could not push busybox to device");
			}

		}
	}
}
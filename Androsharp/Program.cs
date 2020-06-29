using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Androsharp
{
	public static class Program
	{
		private static void Main(string[] args)
		{
			Android.Init();
		}

		static void test()
		{
			var bin =
				"FF D8 FF E0 00 10 4A 46 49 46 00 01 01 01 01 2C " +
				"01 2C 00 00 FF DB 00 43 00 06 04 05 06 05 04 06 " +
				"06 05 06 07 07 06 08 0A 10 0A 0A 09 09 0A 14 0E " +
				"0F 0C 10 17 14 18 18 17 14 16 16 1A 1D 25 1F 1A " + 
				"1B 23 1C 16 16 20 2C 20 23 26 27 29 2A 29 19 1F " +
				"2D 30 2D 28 30 25 28 29 28 FF DB 00 43 01 07 07 " +
				"07 0A 08 0A 13 0A 0A 13 28 1A 16 1A 28 28 28 28 " +
				"28 28 28 28 28 28 28 28 28 28 28 28 28 28 28 28";
			


			var binrg = CliUtilities.ParseByteArray(bin);


			//adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"
			var a  = @"C:\Users\Deci\AppData\Local\Android\Sdk\platform-tools\adb.exe";
			var a2 = "adb.exe";
			var s  = string.Format("exec-out \"dd if=sdcard/image.jpg ibs=128 skip=0 count=1 2>>/dev/null\"");


			var cmd = CliCommand.From("{0} {1}", a2, s);

			var proc = CliResult.Run(cmd, DataType.ByteArray);

			CliUtilities.Compare(binrg, (byte[]) proc.Data);
		}
	}
}
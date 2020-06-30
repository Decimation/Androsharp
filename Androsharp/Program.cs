using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Androsharp.CopyAndConvert;
using Androsharp.Utilities;

namespace Androsharp
{
	public static class Program
	{
		private static void Main(string[] args)
		{
			var cc = new CC_Arguments
			{
				arg_if = "sdcard/image.jpg",
				arg_ibs = 128,
				arg_count = 1,
				arg_skip = 0,
				arg_seek = 0,
				arg_iflag = InputFileFlags.None,
				BinaryRedirect = null,
				StatsRedirect = "sdcard/dd_stats"
			};

			var adb = Android.Value;

			var ddCmd = cc.Compile();

			Console.WriteLine(ddCmd);
		}
	}
}
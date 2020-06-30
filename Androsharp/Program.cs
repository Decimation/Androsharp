using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
		
		[SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH")]
		private static void Main(string[] args)
		{
			var local = @"C:\Users\Deci\Desktop\kino.mkv";
			var dest = @"C:\Users\Deci\Desktop\kino_out.mkv";
			var remote = "sdcard/kino.mkv";
			

			var adb = Android.Value;

			

			var remRg=CopyConvert.ReadRem(remote,dest);

			var eq = CopyConvert.Compare(local, dest);

			Console.WriteLine(eq);
		}
	}
}
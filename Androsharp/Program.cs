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

			var fn = "vid.mkv";
			var remote = "sdcard/vid.mkv";
			
			CopyConvert.Repull(remote);
			
		}
	}
}
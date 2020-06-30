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
	/**
	 * Single file executable build dir
	 * 
	 * C:\Users\Deci\RiderProjects\Androsharp\Androsharp\bin\Release\netcoreapp3.0\win10-x64
	 * C:\Users\Deci\RiderProjects\Androsharp\Androsharp\bin\Release\netcoreapp3.0\win10-x64\publish
	 * C:\Users\Deci\RiderProjects\Androsharp\Androsharp\bin\Debug\netcoreapp3.0\win10-x64
	 *
	 * Single file publish command
	 *
	 * dotnet publish -c Release -r win10-x64
	 *
	 *
	 * Copy build
	 *
	 * copy Androsharp.exe C:\Library /Y
	 * copy Androsharp.exe C:\Users\Deci\Desktop /Y
	 *
	 * Bundle extract dir
	 * 
	 * C:\Users\Deci\AppData\Local\Temp\.net\Androsharp
	 * DOTNET_BUNDLE_EXTRACT_BASE_DIR 
	 */
	public static class Program
	{
		private static void Main(string[] args)
		{
			if (args == null || args.Length < 2) {
				Console.WriteLine("Insufficient arguments");
				return;
			}

			var fn = args[0];

			if (fn == "repull") {
				var remote = args[1];
				
				var bvu = args.Length >= 3 ? args[2] : null;

				if (bvu != null) {
					var bu = bvu.Last().ToString();
					var bv = long.Parse(bvu.SubstringBefore(bu));

					CopyConvert.BlockValue = bv;
					CopyConvert.BlockUnit  = bu;
				}

				var dest = args.Length >= 4 ? args[3] : null;

				CopyConvert.Repull(remote, dest);
			}
			else if (fn == "setup") {
				Android.Setup();
			}
			else if (fn == "reset") {
				Android.Reset();
			}
			else {
				Console.WriteLine("Verb not recognized or implemented: {0}", fn);
			}
		}
	}
}
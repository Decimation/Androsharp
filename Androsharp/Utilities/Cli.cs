using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Androsharp.Utilities
{
	

	public static class Cli
	{
		private const string CMD_EXE = "cmd.exe";

		private const string KERNEL32_DLL = "kernel32.dll";

		private const string ANDROID_SERIAL_ENV = "ANDROID_SERIAL";


		/// <summary>
		/// Creates a <see cref="Process"/> to execute <paramref name="ccmd"/>
		/// </summary>
		/// <param name="ccmd">Command to run</param>
		/// <returns><c>cmd.exe</c> process</returns>
		public static Process Shell(string ccmd)
		{
			Console.WriteLine(">> {0}",ccmd);

			var startInfo = new ProcessStartInfo
			{
				FileName               = CMD_EXE,
				Arguments              = String.Format("/C {0}", ccmd),
				RedirectStandardOutput = true,
				RedirectStandardError  = true,
				UseShellExecute        = false,
				CreateNoWindow         = true,
			};

			var process = new Process
			{
				StartInfo           = startInfo,
				EnableRaisingEvents = true
			};


			return process;
		}

		/// <summary>
		/// http://pinvoke.net/default.aspx/kernel32/SetConsoleOutputCP.html
		/// </summary>
		[DllImport(KERNEL32_DLL, SetLastError = true)]
		private static extern bool SetConsoleOutputCP(
			uint wCodePageId
		);

		/// <summary>
		/// http://pinvoke.net/default.aspx/kernel32/SetConsoleCP.html
		/// </summary>
		[DllImport(KERNEL32_DLL, SetLastError = true)]
		private static extern bool SetConsoleCP(
			uint wCodePageId
		);
	}
}
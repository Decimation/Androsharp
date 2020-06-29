using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Androsharp
{
	class Program
	{
		static void Main(string[] args)
		{
			var bin = "FF D8 FF E0 00 10 4A 46 49 46 00 01 01 01 01 2C 01 2C 00 00 FF DB 00 43 00 06 04 05 06 05 04 06";
			
			Console.WriteLine("Hello World!");
			//adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"
			var a = @"C:\Users\Deci\AppData\Local\Android\Sdk\platform-tools\adb.exe";
			var s = string.Format("exec-out \"dd if=sdcard/image.jpg ibs=128 skip=0 count=1 2>>/dev/null\"");

			var b = read(a, s);
			Console.WriteLine("{0}", b.Length);

			for (int i = 0; i < b.Length; i++) {
				Console.Write("{0:X} ", b[i]);
			}
		}
// http://pinvoke.net/default.aspx/kernel32/SetConsoleOutputCP.html
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleOutputCP(
			uint wCodePageID
		);
		// http://pinvoke.net/default.aspx/kernel32/SetConsoleCP.html
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleCP(
			uint wCodePageID
		);
		public static byte[] ReadToEnd(System.IO.Stream stream)
		{
			long originalPosition = 0;

			if (stream.CanSeek) {
				originalPosition = stream.Position;
				stream.Position  = 0;
			}

			
			
			try {
				byte[] readBuffer = new byte[4096];

				int totalBytesRead = 0;
				int bytesRead;
				

				while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0) {
					totalBytesRead += bytesRead;

					if (totalBytesRead == readBuffer.Length) {
						int nextByte = stream.ReadByte();
						if (nextByte != -1) {
							byte[] temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte) nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}

				byte[] buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead) {
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}

				return buffer;
			}
			finally {
				if (stream.CanSeek) {
					stream.Position = originalPosition;
				}
			}
		}

		static byte[] read(string c, string a)
		{
			// Start the child process.
			Process p = new Process();
			
// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute        = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName               = c;
			p.StartInfo.Arguments              = a;
			// todo: wtf
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
			
			p.Start();
// Do not wait for the child process to exit before
// reading to the end of its redirected stream.
// p.WaitForExit();
// Read the output stream first and then wait.
			//string output = p.StandardOutput.ReadToEnd();
			
			
			
			var b = ReadToEnd(p.StandardOutput.BaseStream);
			p.WaitForExit();

			return b;
		}
	}
}
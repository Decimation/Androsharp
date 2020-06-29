using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Androsharp
{
	public static class CliUtilities
	{
		private const char HEAVY_BALLOT_X   = '\u2718';
		private const char HEAVY_CHECK_MARK = '\u2714';

		private const char MUL_SIGN = '\u00D7';
		private const char RAD_SIGN = '\u221A';

		private const char GT       = '>';
		private const char ASTERISK = '*';

		public static void Compare(byte[] expected, byte[] actual, bool onlyShowMismatches = true)
		{
			var min = actual.Length < expected.Length ? actual : expected;
			var max = actual.Length > expected.Length ? actual : expected;

			Console.WriteLine("Expected length: {0}\nActual length: {1}\n", expected.Length, actual.Length);

			bool allEq = expected.SequenceEqual(actual);
			Console.WriteLine("Full sequence equal: {0}", allEq);

			bool minEq = min.SequenceEqual(actual.Take(min.Length));
			Console.WriteLine("Actual sequence equal: {0} ({1}/{2})\n", minEq, min.Length, max.Length);

			for (int i = 0; i < min.Length; i++) {
				var n1 = expected[i];
				var n2 = actual[i];
				var eq = n1 == n2;
				var ch = eq ? RAD_SIGN : MUL_SIGN;

				var sz = string.Format("[#{0}: {1:X} {2:X} {3}] ", i, n1, n2, ch);

				if (!eq && onlyShowMismatches) {
					Console.Write(sz);
				}
				else if (!onlyShowMismatches) {
					Console.Write(sz);
				}
			}

			Console.WriteLine();
		}

		public static string[] ReadAllLines(StreamReader stream)
		{
			var list = new List<string>();

			while (!stream.EndOfStream) {
				string? line = stream.ReadLine();

				if (line != null) {
					list.Add(line);
				}
			}

			return list.ToArray();
		}

		public static byte[] ParseByteArray(string szPattern)
		{
//			List<byte> patternbytes = new List<byte>();
//			foreach (string szByte in szPattern.Split(' '))
//				patternbytes.Add(szByte == "?" ? (byte) 0x0 : Convert.ToByte(szByte, 16));
//			return patternbytes.ToArray();


			string[] strByteArr   = szPattern.Split(' ');
			var      patternBytes = new byte[strByteArr.Length];

			for (int i = 0; i < strByteArr.Length; i++) {
				patternBytes[i] = Byte.Parse(strByteArr[i], NumberStyles.HexNumber);
			}


			return patternBytes;
		}

		public static byte[] ReadToEnd(Stream stream)
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
	}
}
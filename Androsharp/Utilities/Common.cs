using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
// ReSharper disable InconsistentNaming

namespace Androsharp.Utilities
{
	public static class Common
	{
		internal static readonly HashAlgorithm MD5_Hash = MD5.Create();

		internal static readonly HashAlgorithm Sha1_Hash = SHA1.Create();

		public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
		{
			for (int i = 0; i < locations.Count; i += nSize) {
				yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
			}
		}
		
		public static void AppendAllBytes(string path, byte[] bytes)
		{
			//argument-checking here.

			using (var stream = new FileStream(path, FileMode.Append))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToMegabytes(double d)
		{
			// todo

			const double MUL = 1000;
			
			return d / MUL / MUL;
		}
	}
}
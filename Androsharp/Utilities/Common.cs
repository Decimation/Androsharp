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
		public const int U1 = 1000;
		public const  int U2 = 1024;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToMegabytes(double d)
		{
			// todo

			
			return d / U1 / U1;
		}

		/// <summary>
		///     <returns>String value after [last] <paramref name="a"/></returns>
		/// </summary>
		internal static string SubstringAfter(this string value, string a)
		{
			int posA = value.LastIndexOf(a, StringComparison.Ordinal);
			if (posA == -1)
				return String.Empty;

			int adjustedPosA = posA + a.Length;
			return adjustedPosA >= value.Length ? String.Empty : value.Substring(adjustedPosA);
		}

		/// <summary>
		///     <returns>String value after [first] <paramref name="a"/></returns>
		/// </summary>
		internal static string SubstringBefore(this string value, string a)
		{
			int posA = value.IndexOf(a, StringComparison.Ordinal);
			return posA == -1 ? String.Empty : value.Substring(0, posA);
		}

		/// <summary>
		///     <returns>String value between [first] <paramref name="a"/> and [last] <paramref name="b"/></returns>
		/// </summary>
		internal static string SubstringBetween(this string value, string a, string b)
		{
			int posA = value.IndexOf(a, StringComparison.Ordinal);
			int posB = value.LastIndexOf(b, StringComparison.Ordinal);

			if (posA == -1 || posB == -1)
				return String.Empty;


			int adjustedPosA = posA + a.Length;
			return adjustedPosA >= posB ? String.Empty : value.Substring(adjustedPosA, posB - adjustedPosA);
		}
	}
}
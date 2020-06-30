using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Androsharp.Model;

// ReSharper disable InconsistentNaming

namespace Androsharp.Utilities
{
	internal static class Common
	{
		public const int U1 = 1000;

		public const int U2 = 1024;
		
		internal static readonly HashAlgorithm MD5_Hash = MD5.Create();

		internal static readonly HashAlgorithm Sha1_Hash = SHA1.Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double ToMegabytes(double d)
		{
			// todo

			return d / U1 / U1;
		}

		public static string GetHashString(byte[] hash)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hash)
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		public static byte[] GetFileHash(this HashAlgorithm hash, string s)
		{
			using var destStream = File.OpenRead(s);
			var       hashValue  = hash.ComputeHash(destStream);


			return hashValue;
		}
		public static string GetFileHashString(this HashAlgorithm hash, string s)
		{
			return GetHashString(hash.GetFileHash(s));
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

		internal const string FMT_ARG = "fmt";

		internal static bool SafeIndex<T>(T[] rg, int i, out T t)
		{
			if (i < rg.Length) {
				t= rg[i];
				return true;
			}

			t = default;
			return false;
		}
		
		internal static T SafeIndex<T>(this T[] rg, int i)
		{
			bool b=SafeIndex(rg, i, out var t);

			return t;
		}
	}
}
using System.IO;
using System.Security.Cryptography;

namespace Androsharp.Utilities
{
	public static class Common
	{
		private static readonly HashAlgorithm MD5_Hash = MD5.Create();

		private static readonly HashAlgorithm Sha1_Hash = SHA1.Create();

		public static FileMetaInfo ReadInfo(string filename)
		{
			// CRC32, MD5, SHA-1

			var stream = File.OpenRead(filename);

			var fileBytes = File.ReadAllBytes(filename);

			var fmi = new FileMetaInfo(MD5_Hash.ComputeHash(stream),
			                           Sha1_Hash.ComputeHash(stream),
			                           fileBytes, fileBytes.Length);


			return fmi;
		}
	}
}
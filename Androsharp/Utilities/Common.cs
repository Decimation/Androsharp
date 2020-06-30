using System.IO;
using System.Security.Cryptography;

namespace Androsharp.Utilities
{
	public static class Common
	{
		internal static readonly HashAlgorithm MD5_Hash = MD5.Create();

		internal static readonly HashAlgorithm Sha1_Hash = SHA1.Create();

		
	}
}
using System;
using System.Diagnostics;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Androsharp.Utilities
{
	public sealed class FileMetaInfo : IEquatable<FileMetaInfo>
	{
		public FileMetaInfo(byte[] md5, byte[] sha1, byte[] data, long size)
		{
			MD5  = md5;
			SHA1 = sha1;
			Data = data;
			Size = size;
		}

		public byte[] MD5 { get; }

		public byte[] SHA1 { get; }

		public byte[] Data { get; }

		public long Size { get; }

		public bool Equals(FileMetaInfo other)
		{
			return MD5.SequenceEqual(other.MD5)
			       && SHA1.SequenceEqual(other.SHA1)
			       && Data.SequenceEqual(other.Data)
			       && Size == other.Size;
		}

		public override bool Equals(object obj)
		{
			return obj is FileMetaInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(MD5, SHA1, Data, Size);
		}

		public static bool operator ==(FileMetaInfo left, FileMetaInfo right)
		{
			
			return left.Equals(right);
		}

		public static bool operator !=(FileMetaInfo left, FileMetaInfo right)
		{
			return !left.Equals(right);
		}
	}
}
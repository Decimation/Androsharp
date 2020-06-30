#region

using System;
using System.Text;
using Androsharp.Model;
using Androsharp.Utilities;

#endregion

// ReSharper disable InconsistentNaming

#nullable enable

namespace Androsharp.CopyAndConvert
{
	// todo


	/// <summary>
	///     DD arguments
	/// </summary>
	public sealed class CCArguments : ICommand
	{
		public CCArguments()
		{
			InputFile      = null!;
			InputBlockSize = 0;
			Count          = 0;
			Seek           = 0;
			Skip           = 0;
			InputFileFlag  = InputFileFlags.None;


			BinaryRedirect        = null;
			StatsRedirect         = "sdcard/dd_stats";
			FileDescriptorReplace = true;
		}

		/// <summary>
		///     <c>if</c>
		/// </summary>
		public string InputFile { get; internal set; }


		/// <summary>
		///     <c>ibs</c>
		/// </summary>
		public long InputBlockSize { get; internal set; }

		/// <summary>
		///     <c>count</c>
		/// </summary>
		public int Count { get; internal set; }

		/// <summary>
		///     <c>skip</c>
		/// </summary>
		public int Skip { get; internal set; }

		/// <summary>
		///     <c>seek</c>
		/// </summary>
		public int Seek { get; internal set; }

		/// <summary>
		///     <c>iflag</c>
		/// </summary>
		public InputFileFlags InputFileFlag { get; internal set; }

		/// <summary>
		///     <see cref="Unix.FD_STDOUT" /> - DD data
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public string? BinaryRedirect { get; internal set; }

		/// <summary>
		///     <see cref="Unix.FD_STDERR" /> - DD stats
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public string? StatsRedirect { get; internal set; }

		/// <summary>
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public bool FileDescriptorReplace { get; internal set; }

		public string Compile()
		{
			// adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"

			var sb = new StringBuilder();


			sb.AppendFormat("dd if={0} ibs={1} skip={2} count={3} seek={4}",
			                InputFile, InputBlockSize + CopyConvert.BlockValue, Skip, Count, Seek);

			if (InputFileFlag != InputFileFlags.None) {
				sb.AppendFormat((string) " iflag={0}", GetFlagString(InputFileFlag));
			}

			string fileDescriptorOp = FileDescriptorReplace ? Unix.REPLACE_OP : Unix.APPEND_OP;

			sb.Append(GetRedirectString(BinaryRedirect, CopyConvert.FD_DD_BINARY, fileDescriptorOp));

			sb.Append(GetRedirectString(StatsRedirect, CopyConvert.FD_DD_STATS, fileDescriptorOp));


			return sb.ToString();
		}

		private static string GetRedirectString(string? redirect, int fd, string fileDescriptorOp)
		{
			var sb = new StringBuilder();
			if (redirect != null) {
				sb.Append(" ").Append(fd).Append(fileDescriptorOp).Append(redirect);
			}

			return sb.ToString();
		}

		private static string GetFlagString(InputFileFlags flags)
		{
			switch (flags) {
				case InputFileFlags.None:
					return null;
					break;
				case InputFileFlags.SkipBytes:
					return "skip_bytes";
					break;
				case InputFileFlags.FullBlock:
					return "fullblock";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
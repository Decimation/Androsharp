using System.Text;
using Androsharp.Model;
using Androsharp.Utilities;

// ReSharper disable InconsistentNaming

#nullable enable

namespace Androsharp.CopyAndConvert
{
	// todo


	/// <summary>
	/// DD arguments
	/// </summary>
	public sealed class CC_Arguments : ICommand
	{
		public string arg_if { get; internal set; }

		public long arg_ibs { get; internal set; }

		public int arg_count { get; internal set; }

		public int arg_skip { get; internal set; }

		public int arg_seek { get; internal set; }


		public InputFileFlags arg_iflag { get; internal set; }

		/// <summary>
		/// 
		/// <see cref="UnixFileDescriptor.StdOut"/> - DD data
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public string? BinaryRedirect { get; internal set; }

		/// <summary>
		/// 
		/// <see cref="UnixFileDescriptor.StdErr"/> - DD stats
		/// 
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public string? StatsRedirect { get; internal set; }

		/// <summary>
		/// 
		/// 
		/// </summary>
		/// <remarks>Auxiliary special argument</remarks>
		public bool FileDescriptorReplace { get; internal set; }

		

		public CC_Arguments()
		{
			arg_if = null!;
			arg_ibs = 0;
			arg_count = 0;
			arg_seek = 0;
			arg_skip = 0;
			arg_iflag = InputFileFlags.None;
			
			
			BinaryRedirect = null;
			StatsRedirect = "sdcard/dd_stats";
			FileDescriptorReplace = true;

		}

		public string Compile()
		{
			// adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"

			var sb = new StringBuilder();
			
			
			sb.AppendFormat("dd if={0} ibs={1} skip={2} count={3} seek={4}",
			                arg_if, arg_ibs, arg_skip, arg_count, arg_seek);

			if (arg_iflag != InputFileFlags.None) {
				sb.AppendFormat(" iflag={0}", arg_iflag.ToString());
			}

			var fileDescriptorOp = FileDescriptorReplace ? UnixConstants.REPLACE_OP : UnixConstants.APPEND_OP;

			HandleRedirect(BinaryRedirect, CopyConvert.FD_DD_BINARY);

			HandleRedirect(StatsRedirect, CopyConvert.FD_DD_STATS);
			
			
			void HandleRedirect(string? redirect, UnixFileDescriptor fd)
			{
				if (redirect != null) {
					sb.Append(" ").Append((int) fd).Append(fileDescriptorOp).Append(redirect);
				}
			}


			return sb.ToString();
		}

		
	}
}
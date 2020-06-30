using Androsharp.Model;
using Androsharp.Utilities;

namespace Androsharp.CopyAndConvert
{
	// https://github.com/Decimation/SmartBridge
	// https://github.com/Decimation/SmartBridge/blob/master/SmartBridge/Bridge.cs

	// usage: dd [if=FILE] [of=FILE] [ibs=N] [obs=N] [bs=N] [count=N] [skip=N]
	//         [seek=N] [conv=notrunc|noerror|sync|fsync] [status=noxfer|none]
	// 
	// Copy/convert files.
	// 
	// if=FILE         Read from FILE instead of stdin
	// of=FILE         Write to FILE instead of stdout
	// bs=N            Read and write N bytes at a time
	// ibs=N           Read N bytes at a time
	// obs=N           Write N bytes at a time
	// count=N         Copy only N input blocks
	// skip=N          Skip N input blocks
	// seek=N          Skip N output blocks
	// conv=notrunc    Don't truncate output file
	// conv=noerror    Continue after read errors
	// conv=sync       Pad blocks with zeros
	// conv=fsync      Physically write data out before finishing
	// status=noxfer   Don't show transfer rate
	// status=none     Don't show transfer rate or records in/out
	// 
	// Numbers may be suffixed by c (*1), w (*2), b (*512), kD (*1000), k (*1024),
	// MD (*1000*1000), M (*1024*1024), GD (*1000*1000*1000) or G (*1024*1024*1024).

	// dd if={0} bs={1} skip={2} 2>>/dev/null
	// remote, buffer, block
	// 
	// https://gist.github.com/alopatindev/e94ff95ea834500abe2da81ac2a7764f

	// adb shell ls -l {arg}
	// Example output:
	// -rw-rw---- 1 root sdcard_rw 6000000000 2020-04-05 17:47 sdcard/bigfile

	/*
	BusyBox v1.31.1-meefik (2020-03-11 18:59:07 UTC) multi-call binary.
	Usage: dd [if=FILE] [of=FILE] [ibs=N obs=N/bs=N] [count=N] [skip=N] [seek=N]
			[conv=notrunc|noerror|sync|fsync]
			[iflag=skip_bytes|fullblock] [oflag=seek_bytes|append]
	Copy a file with converting and formatting
			if=FILE					Read from FILE instead of stdin
			of=FILE					Write to FILE instead of stdout
			bs=N					Read and write N bytes at a time
			ibs=N					Read N bytes at a time
			obs=N					Write N bytes at a time
			count=N					Copy only N input blocks
			skip=N					Skip N input blocks
			seek=N					Skip N output blocks
			conv=notrunc			Don't truncate output file
			conv=noerror			Continue after read errors
			conv=sync				Pad blocks with zeros
			conv=fsync				Physically write data out before finishing
			conv=swab				Swap every pair of bytes
			iflag=skip_bytes        skip=N is in bytes
			iflag=fullblock			Read full blocks
			oflag=seek_bytes        seek=N is in bytes
			oflag=append			Open output file in append mode
			status=noxfer			Suppress rate output
			status=none				Suppress all output
	N may be suffixed by c (1), w (2), b (512), kB (1000), k (1024), MB, M, GB, G
	*/


	// Possible solutions
	// 1.	Create block segments with dd, read from exec-out stdout, and collate the raw binary
	//		to reassemble the original file
	//	  
	//	  
	// 2.	Create block segments with dd, write to remote file, then pull to local and collate the raw binary
	//		files to reassemble the original file
	
	
	/// <summary>
	/// DD utilities
	/// </summary>
	public class CopyConvert
	{
		// adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"

		public const UnixFileDescriptor FD_DD_BINARY = UnixFileDescriptor.StdOut;
		
		
		public const UnixFileDescriptor FD_DD_STATS = UnixFileDescriptor.StdErr;


		public CliCommand Create(CC_Arguments args)
		{
			var ddCmdStr = args.Compile();
			var ddCmd = CliCommand.Create(Scope.AdbExecOut, ddCmdStr);

			
			
			
			// todo - left off here



			return ddCmd;
		}
	}
}
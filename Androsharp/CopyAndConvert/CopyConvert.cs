using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Androsharp.Model;
using Androsharp.Utilities;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

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
	public static class CopyConvert
	{
		// adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"

		public const UnixFileDescriptor FD_DD_BINARY = UnixFileDescriptor.StdOut;

		public const UnixFileDescriptor FD_DD_STATS = UnixFileDescriptor.StdErr;

		/// <summary>
		/// Block size
		/// </summary>
		private const long BlockSize = 8192 * 256;

		
		
		
		public static CC_Record Create(CC_Arguments args)
		{
			var ddCmdStr = args.Compile();
			ddCmdStr = string.Format("\"{0}\"", ddCmdStr);

			var ddCmd = CliCommand.Create(Scope.AdbExecOut, ddCmdStr);


			var ddRes = CliResult.Run(ddCmd, DataType.ByteArray);

			var ccRec = new CC_Record();
			ccRec.BinaryRaw = (byte[]) ddRes.Data;
			//ccRec.StatsRaw  = Android.Value.ReadFile(args.StatsRedirect);


			// todo - left off here


			return ccRec;
		}

		private static long SizeToBlocks(long size, long buf)
		{
			return (long) Math.Ceiling((double) (size / buf));
		}

		public static bool Compare(string control, string ret)
		{
			var fmiCtrl = Common.ReadInfo(control);
			var fmiRet  = Common.ReadInfo(ret);



			var sizeEq = fmiCtrl.Size == fmiRet.Size;
			Console.WriteLine("Size equal: {0}", sizeEq);

			var md5Eq = fmiCtrl.MD5.SequenceEqual(fmiRet.MD5);
			Console.WriteLine("Md5 equal: {0}", md5Eq);
			
			var sha1Eq = fmiCtrl.SHA1.SequenceEqual(fmiRet.SHA1);
			Console.WriteLine("Sha1 equal: {0}", sha1Eq);
			
			var dataEq = fmiCtrl.Data.SequenceEqual(fmiRet.Data);
			Console.WriteLine("Data equal: {0}", dataEq);
			
			return fmiCtrl == fmiRet;
		}

		public static byte[] ReadRem(string rem, string dest)
		{
			long remSize = Android.Value.RemoteSize(rem);
			long nBlocks = SizeToBlocks(remSize, BlockSize);

			var rg = new List<byte>();

			for (int i = 0; i <= nBlocks; i++) {
				var ccArg = new CC_Arguments
				{
					arg_count = 1,
					arg_ibs   = BlockSize,
					arg_if    = rem,
					arg_skip  = i,
				};


				var res = Create(ccArg);

				rg.AddRange(res.BinaryRaw);

				Console.Write("\r{0}/{1}", i, nBlocks);
			}

			Console.WriteLine();

			var brg = rg.ToArray();

			File.WriteAllBytes(dest, brg);

			return brg;
		}
	}
}
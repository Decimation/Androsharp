using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Androsharp.Model;
using Androsharp.Utilities;

using static Androsharp.Utilities.Common;

// ReSharper disable InconsistentNaming

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

		public const int FD_DD_BINARY = Unix.FD_StdOut;

		public const int FD_DD_STATS = Unix.FD_StdErr;


		/// <summary>
		/// Maximum recommended block size
		/// </summary>
		public static long BlockSizeMaxBytes { get; } = 8192 * 4096;

		/// <summary>
		/// Block size
		/// </summary>
		public static long BlockUnits { get; set; } = 16;

		public static string BlockSizeUnit { get; set; } = "M";

		/// <summary>
		/// Block size
		/// </summary>
		public static long BlockSize {
			get {
				
				// N may be suffixed by c (1), w (2), b (512), kB (1000), k (1024), MB, M, GB, G
				
				long mul = BlockSizeUnit switch
				{
					"c" => 1,
					"w" => 2,
					"b" => 512,
					"kB" => U1,
					"k" => U2,
					"MB" => U1 * U1,
					"M" => U2 * U2,
					"GB" => U1 * U1 * U1,
					"G" => U2 * U2 * U2,
					_ => 1
				};

				return BlockUnits * mul;
			}
		}

		private static string DefaultOutput {
			get {
#if DEBUG
				return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif
				return Environment.CurrentDirectory;
			}
		}

		public static CCRecord ReadRecord(CCArguments args)
		{
			var ddCmdStr = args.Compile();
			ddCmdStr = string.Format("\"{0}\"", ddCmdStr);

			var ddCmd = CliCommand.Create(Scope.AdbExecOut, ddCmdStr);


			var ddRes = CliResult.Run(ddCmd, DataType.ByteArray);

			var ccRec = new CCRecord();

			lock (ccRec) {
				ccRec.BinaryRaw = (byte[]) ddRes.Data;
				//ccRec.StatsRaw = Android.Value.ReadFile(args.StatsRedirect);
			}

			//ddRes.CommandProcess.Close();
			//ddRes.CommandProcess.WaitForExit();
			//ddRes.CommandProcess.Kill();


			return ccRec;
		}

		private static long SizeToBlocks(long size, long buf) => (long) Math.Ceiling((double) (size / buf));

		public static void Repull(string rem)
		{
			var fn  = Path.GetFileNameWithoutExtension(rem);
			var ext = Path.GetExtension(rem);

			var nn = fn + "_out" + ext;

			var dest = Path.Combine(DefaultOutput, nn);


			Repull(rem, dest);
		}

		/// <summary>
		/// Repull implementation
		/// </summary>
		public static void Repull(string rem, string dest)
		{
			if (BlockSize >= BlockSizeMaxBytes) {
				Console.WriteLine("Warning: Using a block size larger than the recommended maximum");
			}

			Console.WriteLine("{0} >> {1}", rem, dest);

			if (File.Exists(dest)) {
				File.Delete(dest);
			}

			long remSize = Android.RemoteSize(rem);
			long nBlocks = SizeToBlocks(remSize, BlockSize);

			if (BlockSize >= remSize) {
				nBlocks = 1;
			}

			Console.WriteLine();
			Console.WriteLine("Remote size: {0} bytes", remSize);
			Console.WriteLine("Block size: {0} bytes ({1} bu)\n", BlockSize, BlockUnits);

			double rateSum = 0;

			const int RND = 2;

			int rgi = 0;

			var stream = new FileStream(dest, FileMode.Append);

			for (int i = 0; i <= nBlocks; i++) {
				var start = DateTimeOffset.Now;

				var ccArg = new CCArguments
				{
					arg_count = 1,
					arg_ibs   = BlockSize,
					arg_if    = rem,
					arg_skip  = i,
				};


				var res    = ReadRecord(ccArg);
				var resBin = res.BinaryRaw;

				lock (resBin) {
					// ~123ms
					stream.Write(resBin, 0, resBin.Length);
					stream.Flush();
					rgi += resBin.Length;
				}


				var end = DateTime.Now;

				var duration = end - start;


				var bytesPerSec     = resBin.Length / duration.TotalSeconds;
				var megabytesPerSec = Math.Round(Common.ToMegabytes(bytesPerSec), RND);

				rateSum += megabytesPerSec;

				var avgMegabytesPerSec = Math.Round(rateSum / i, RND);

				var percent = ((double) rgi) / remSize;

				Console.Write("\r{0}/{1} ({2} MB/sec) ({3} MB/sec avg) ({4:P})",
				              i, nBlocks, megabytesPerSec, avgMegabytesPerSec, percent);


				//Console.Clear();
			}

			stream.Flush();
			stream.Close();

			Console.WriteLine();

			Console.WriteLine("Wrote data to {0}", dest);
		}
	}
}
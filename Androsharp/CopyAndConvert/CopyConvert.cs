#region

#nullable enable
using System;
using System.IO;
using Androsharp.Model;
using Androsharp.Utilities;
using static Androsharp.Utilities.Common;

#endregion

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
	///     DD utilities
	/// </summary>
	public static class CopyConvert
	{

		static CopyConvert()
		{
			Android.Setup();
		}
		
		
		// adb shell "dd if=sdcard/image.jpg bs=128 skip=0 count=1 2>>/dev/null"

		public const int FD_DD_BINARY = Unix.FD_STDOUT;

		public const int FD_DD_STATS = Unix.FD_STDERR;


		/// <summary>
		///     Maximum recommended block size tested
		/// </summary>
		public const long BlockSizeMaxBytes = 8192 * 4096;

		/// <summary>
		///     Block size value that will be multiplied by the <see cref="BlockUnit" /> unit multiple to calculate
		///     <see cref="BlockSize" />
		/// </summary>
		public static long BlockValue { get; set; } = 16;

		/// <summary>
		///     Block size data unit that will determine the multiple by which <see cref="BlockValue" /> is multiplied
		///     to calculate <see cref="BlockSize" />
		/// </summary>
		public static string BlockUnit { get; set; } = "M";

		/// <summary>
		///     Block size: <see cref="BlockValue" /> <c>*</c> <see cref="BlockUnit" /> multiple <c>=</c>
		///     <see cref="BlockSize" /> bytes
		/// </summary>
		public static long BlockSize {
			get {
				// N may be suffixed by c (1), w (2), b (512), kB (1000), k (1024), MB, M, GB, G

				long mul = BlockUnit switch
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

				return BlockValue * mul;
			}
		}

		public static string DefaultOutput {
			get {
#if DEBUG
				return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif
				return Environment.CurrentDirectory;
			}
		}

		public static bool RunChecks { get; set; } = true;
		
		private static CCRecord ReadRecord(CCArguments args)
		{
			string ddCmdStr = args.Compile();
			ddCmdStr = String.Format("\"{0}\"", ddCmdStr);

			var ddCmd = CliCommand.Create(CliScope.AdbExecOut, ddCmdStr);


			var ddRes = CliResult.Run(ddCmd, CliDataType.ByteArray);

			var ccRec = new CCRecord();

			lock (ccRec) {
				ccRec.BinaryRaw = ddRes.ByteArray;
				//ccRec.StatsRaw = Android.Value.ReadFile(args.StatsRedirect);
			}

			//ddRes.CommandProcess.Close();
			//ddRes.CommandProcess.WaitForExit();
			//ddRes.CommandProcess.Kill();


			return ccRec;
		}

		private static long SizeToBlocks(long size, long blockSize) => (long) Math.Ceiling((double) (size / blockSize));
		

		/// <summary>
		///     Repull implementation
		/// </summary>
		public static void Repull(string remote, string? dest = null)
		{
			if (dest == null) {
				string? fn  = Path.GetFileNameWithoutExtension(remote);
				string? ext = Path.GetExtension(remote);

				string nn = fn + "_out" + ext;

				dest = Path.Combine(DefaultOutput, nn);
			}
			
			if (BlockSize >= BlockSizeMaxBytes) {
				Console.WriteLine("Warning: Using a block size larger than the recommended maximum");
			}

			if (File.Exists(dest)) {
				File.Delete(dest);
			}

			long remSize = Android.GetFileSize(remote);
			long nBlocks = SizeToBlocks(remSize, BlockSize);

			if (BlockSize >= remSize) {
				nBlocks = 1;
			}

			Console.WriteLine("\nRemote file: {0}", remote);
			Console.WriteLine("Remote size: {0} bytes", remSize);

			Console.WriteLine("\nBlock size: {0} bytes ({1} bv, {2} bu)", BlockSize, BlockValue, BlockUnit);
			
			Console.WriteLine("\nDestination file: {0}", dest);
			
			
			double rateSum = 0;

			const int RND = 2;

			int nbytes = 0;

			var outStream = new FileStream(dest, FileMode.Append);

			for (int i = 0; i <= nBlocks; i++) {
				
				var start = DateTimeOffset.Now;

				var ccArg = new CCArguments
				{
					Count = 1,
					InputBlockSize   = BlockSize,
					InputFile    = remote,
					Skip  = i,
				};


				var    res    = ReadRecord(ccArg);
				byte[] resBin = res.BinaryRaw;

				lock (resBin) {
					// ~123ms
					outStream.Write(resBin, 0, resBin.Length);
					outStream.Flush();
					nbytes += resBin.Length;
				}


				var end = DateTime.Now;

				var duration = end - start;
				
				double bytesPerSec     = resBin.Length / duration.TotalSeconds;
				double megabytesPerSec = Math.Round(ToMegabytes(bytesPerSec), RND);

				rateSum += megabytesPerSec;

				double avgMegabytesPerSec = Math.Round(rateSum / i, RND);

				double percent = (double) nbytes / remSize;

				Console.Write("\r{0}/{1} ({2} MB/sec) ({3} MB/sec avg) ({4:P})",
				              i, nBlocks, megabytesPerSec, avgMegabytesPerSec, percent);


				//Console.Clear();
			}

			outStream.Flush();
			outStream.Close();

			Console.WriteLine("\n\nWrote data to {0}!\n\n", dest);

			if (RunChecks) {
				var destSize = new FileInfo(dest).Length;
				
				var destSha1 = Sha1_Hash.GetFileHashString(dest);

				var remSha1 = Android.GetFileSha1Hash(remote);

				Console.WriteLine("Size equal: {0} | Sha1 equal: {1}", 
				                  CliUtilities.GetSuccessChar(destSize, remSize),
				                  CliUtilities.GetSuccessChar(destSha1, remSha1));
				
			}
		}
	}
}
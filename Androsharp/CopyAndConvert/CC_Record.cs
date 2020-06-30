using System;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Androsharp.CopyAndConvert
{
	/// <summary>
	/// DD record
	/// </summary>
	public sealed class CC_Record
	{
		/// <summary>
		///
		/// <see cref="CopyConvert.FD_DD_STATS"/>
		/// </summary>
		public string StatsRaw { get; internal set; }
		
		/// <summary>
		///
		/// <see cref="CopyConvert.FD_DD_BINARY"/>
		/// </summary>
		public byte[] BinaryRaw { get; internal set; }
		
	}
}
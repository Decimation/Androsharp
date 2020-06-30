namespace Androsharp.CopyAndConvert
{
	public class CC_Record
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
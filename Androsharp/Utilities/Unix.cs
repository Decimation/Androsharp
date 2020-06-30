namespace Androsharp.Utilities
{
	public static class Unix
	{
		public const string REPLACE_OP = ">";
		
		public const string APPEND_OP = ">>";
		
		public const string DEV_NULL = "/dev/null";

		/// <summary>
		/// File descriptor for standard input
		/// </summary>
		public const int FD_STDIN = 0;
		
		/// <summary>
		/// File descriptor for standard output
		/// </summary>
		public const int FD_STDOUT = 1;
		
		/// <summary>
		/// File descriptor for standard error
		/// </summary>
		public const int FD_STDERR = 2;
	}
}
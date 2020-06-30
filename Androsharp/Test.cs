using System;
using Androsharp.CopyAndConvert;
using Androsharp.Model;
using Androsharp.Utilities;

namespace Androsharp
{
	internal struct TestInfo
	{
		internal string local { get; set; }
		internal string remote { get; set; }
		internal string dest { get; set; }
	}
	
	internal static class Test
	{
		internal static bool RunFileTest(TestInfo t)
		{
			
			CopyConvert.Repull(t.remote,t.dest);

			var localInfo = FileMetaInfo.Get(t.local);
			var destInfo  = FileMetaInfo.Get(t.dest);
			
			var eq        = localInfo == destInfo;


			return eq;
		}
		
	}
}
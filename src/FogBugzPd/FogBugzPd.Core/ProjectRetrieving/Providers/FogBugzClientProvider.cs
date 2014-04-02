using FogBugzPd.Core;
using FogLampz;

namespace FogBugzPd.Core.ProjectRetrieving
{
	public class FogBugzClientProvider : IFogBugzClientProvider
	{
		public FogBugzClientEx GetClient(string key)
		{
			// get  client from cache
			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, key);

			object tmpObj = null;
			MsCache.TryGet(cacheKey, ref tmpObj);

			return tmpObj as FogBugzClientEx;
		}
	}
}
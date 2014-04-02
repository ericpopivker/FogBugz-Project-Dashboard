using System;
using FogBugzPd.Core;

namespace FogBugzPd.Core.ProjectRetrieving
{
	public class CachingProvider : ICachingProvider
	{
		public T Retrieve<T>(string key)
		{
			object storedValue = null;
			if (MsCache.TryGet(key, ref storedValue))
				return (T)storedValue;

			return default(T);
		}

		public void Store(string key, object value, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
		{
			MsCache.Set(key, value, absoluteExpiration, slidingExpiration);
		}

		public string GetCacheKey(string key)
		{
			return string.Format("{0}.ProjectListValidCombinations", key);
		}

		public void Remove(string key)
		{
			MsCache.TryRemove(key);
		}
	}
}
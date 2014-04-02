using System;

namespace FogBugzPd.Core.ProjectRetrieving
{
	public interface ICachingProvider
	{
		T Retrieve<T>(string key);
		void Store(string key, object value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);

		string GetCacheKey(string key);

		void Remove(string key);
	}
}
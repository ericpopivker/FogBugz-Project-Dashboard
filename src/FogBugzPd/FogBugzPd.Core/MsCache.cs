using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using StackExchange.Profiling;

namespace FogBugzPd.Core
{
	public class MsCache
	{
		private static Cache SystemCache
		{
			get { return HttpRuntime.Cache; }
		}

		
		public static void Set(string key, object value)
		{
			Set(key, value, null);
		}

		public static void Set(string key, object value, TimeSpan? absoluteExpiration)
		{
			Set(key, value, absoluteExpiration, null);
		}

		public static void Set(string key, object value, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
		{
			//ASP.NET Cache doesn't allow nulls so use DBNull instead
			if (value == null)
				value = DBNull.Value;

			if (absoluteExpiration != null)
				SystemCache.Insert(key, value, null, DateTime.UtcNow.Add(absoluteExpiration.Value), Cache.NoSlidingExpiration);
			else if (slidingExpiration != null)
				SystemCache.Insert(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration.Value);
			else
				SystemCache.Insert(key, value);
		}

		public static bool Exists(string key)
		{
			object value = null;
			return TryGet(key, ref value);
		}

		public static bool TryGet(string key, ref object value)
		{
			object o = null;
			
			using (MiniProfiler.Current.Step("Load from MSCache (key:" + key+ ")"))
			{
				o = SystemCache.Get(key);
			}
				if (o == null)
					return false;

				value = o == DBNull.Value ? null : o;

				return true;
			
		}

		public static bool TryRemove(string key)
		{
			object o = SystemCache.Remove(key);

			if (o == null)
				return false;

			return true;
		}

		public static void Clear()
		{
			var keys = new List<string>();

			// retrieve application Cache enumerator
			IDictionaryEnumerator enumerator = SystemCache.GetEnumerator();

			// copy all keys that currently exist in Cache
			while (enumerator.MoveNext())
				keys.Add(enumerator.Key.ToString());

			// delete every key from cache
			for (int i = 0; i < keys.Count; i++)
				SystemCache.Remove(keys[i]);

		}

		public static int Count()
		{
			return SystemCache.Count;
		}

		public static DateTime GetObjectExpirationTime(string key)
		{
			//from http://stackoverflow.com/questions/344479/how-can-i-get-the-expiry-datetime-of-an-httpruntime-cache-object
			object cacheEntry = SystemCache.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(SystemCache, new object[] { key, 1 });
			
			PropertyInfo utcExpiresProperty = cacheEntry.GetType().GetProperty("UtcCreated", BindingFlags.NonPublic | BindingFlags.Instance);
			DateTime utcExpiresValue = (DateTime)utcExpiresProperty.GetValue(cacheEntry, null);

			return utcExpiresValue;
		}
	}
}
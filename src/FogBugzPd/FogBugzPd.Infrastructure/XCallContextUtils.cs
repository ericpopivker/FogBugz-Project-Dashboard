using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FogBugzPd.Infrastructure
{
	using System.Runtime.Remoting.Messaging;
	using System.Web;

	/// <summary>
	/// Thread safe user context that works in both: Web and non Web environments
	/// </summary>
	public static class XCallContextUtils
	{
		/// <summary>
		/// Save data into appropriate storage (Web Context or Local thread storage)
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetData(string key, object value)
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null)
				CallContext.SetData(key, value);
			else
				httpContext.Items[key] = value;
		}

		/// <summary>
		/// Retreive data from appropriate storage.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static object GetData(string key)
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null)
				return CallContext.GetData(key);
			else
				return httpContext.Items[key];
		}

		/// <summary>
		/// Remove data identified by key from storage.
		/// </summary>
		/// <param name="key"></param>
		public static void RemoveData(string key)
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null)
				CallContext.FreeNamedDataSlot(key);
			else
				httpContext.Items.Remove(key);
		}
	}

}

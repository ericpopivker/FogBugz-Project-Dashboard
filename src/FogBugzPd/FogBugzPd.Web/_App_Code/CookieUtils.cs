using System;
using System.ComponentModel;
using System.Web;

namespace FogBugzPd.Web
{
	public enum Cookies
	{
	}

	public class CookieUtils
	{
		public static T? GetCookie<T>(Cookies type) where T : struct
		{
			var cookie = HttpContext.Current.Request.Cookies[type.ToString()];
			if (cookie == null || cookie.Value==null) return null;

			var converter = TypeDescriptor.GetConverter(typeof (T));

			var result = (T)converter.ConvertFrom(cookie.Value);
			return result;
		}

		public static void SetCookie<T>(Cookies type, T value, DateTime expireAt)
		{
			var cookie = new HttpCookie(type.ToString(), value.ToString());
			cookie.Expires = expireAt;

			HttpContext.Current.Response.Cookies.Add(cookie);
		}

		public static void ClearCookie(Cookies type)
		{
			var cookie = HttpContext.Current.Request.Cookies[type.ToString()];
			if (cookie == null) return;
			cookie.Expires = DateTime.Now.AddYears(-1);
			HttpContext.Current.Response.Cookies.Add(cookie);
		}
	}
}
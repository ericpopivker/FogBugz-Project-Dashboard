using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FogBugzPd.Web.Utils
{
	public class UrlUtils
	{
		public static string LoginUrl()
		{
			var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
			return helper.Action("Login", "Account");
		}

		public static string AddParamToUrl(string url, string paramName, object paramValue)
		{
			if (url.Contains("?"))
			{
				url += String.Format("&{0}={1}", paramName, paramValue);
			}
			else
			{
				url += String.Format("?{0}={1}", paramName, paramValue);
			}
			return url;
		}

		public static string GenFogBugzApiUrl(string fogBugzUrl)
		{
			return fogBugzUrl + "/api.asp";
		}

		public static string GetSeoFriendly(string title)
		{
			var badChars = new[]
							   {
								   '!', '@', '#', '$', '%', '_', '.', ',', ':', '?', ';', '"', '\'', '&', '(', ')', '[',
								   ']'
							   };
			title =
				String.Concat(Regex.Unescape(title).ToLower().Trim().Split(badChars,
																		   StringSplitOptions.RemoveEmptyEntries));
			title = title.Replace(" ", "-");
			title = title.Replace("  ", "-");
			title = title.Replace("+", "-"); //Eric - '+' breaks the url
			title = title.Replace("/", "-");
			title = title.Replace("--", "-");
			return title;
		}

		public static string EncodeUrlParam(string value)
		{
			if (String.IsNullOrEmpty(value)) return null;

			return GetSeoFriendly(value);
		}

	}
}

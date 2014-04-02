using System.Configuration;
using System.Web.Mvc;

namespace FogBugzPd.Web
{
	public static class UrlExtensions
	{
		public static string ContentV(this UrlHelper urlHelper, string contentPath)
		{
			string absolutePath = urlHelper.Content(contentPath);

			string version = ConfigurationManager.AppSettings["UrlContentVersion"];

			if (string.IsNullOrEmpty(version))
				return absolutePath;

			if (absolutePath.Contains("?"))
				absolutePath += "&";
			else
				absolutePath += "?";

			absolutePath += "v=" + version;
			return absolutePath;
		}

	}
}
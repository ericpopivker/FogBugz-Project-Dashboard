using System;
using FogLampz.Model;

namespace FogBugzPd.Web.Utils
{
	public class FogBugzUrlUtils : UrlUtils
	{
		public static string GetGridUrl(FixFor fixFor, Case parentCase, Status status)
		{
			var url = UserContext.FogBugzUrl + "/default.asp?sView=grid-outline";
			if (fixFor != null)
				url = AddParamToUrl(url, "ixFixFor", fixFor.Index);
			if (parentCase != null)
				url = AddParamToUrl(url, "searchFor", String.Format("parent=\"{0}\"", parentCase.Index));
			if (status != null)
				url = AddParamToUrl(url, "ixStatus", status.Index);

			return url;
		}

		public static object GetWikiUrl(object wikiPageId)
		{
			return String.Format("{0}/default.asp?{1}", UserContext.FogBugzUrl, wikiPageId);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Core;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Web._App_Code.Extensions
{
	public static class HtmlExtensions
	{
		public static HtmlString Link(this HtmlHelper helper, string caption, string url, IDictionary<string, object> attributes = null)
		{
			var builder = new TagBuilder("a");
			builder.SetInnerText(caption);
			builder.Attributes.Add("href", url);
			builder.MergeAttributes(attributes);

			return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
		}

		public static HtmlString RefreshData(this HtmlHelper helper, DateTime creationTime, int? projectId, int? milestoneId, int? subProjectParentCaseId, MsCacheDataType dataType)
		{
			var builder = new TagBuilder("a");
			builder.SetInnerText("Refresh Data");
			builder.Attributes.Add("href", "#");

			var onclickCode = projectId.HasValue ? String.Format("clearCache({0}, {1}, {2}, {3});return false;", (int)dataType, projectId, milestoneId, subProjectParentCaseId.HasValue ? subProjectParentCaseId.ToString() : "null") : String.Format("clearCache({0})", (int)dataType);

			builder.Attributes.Add("onclick", onclickCode);

			var text = String.Format("Data From: {0}&nbsp;", creationTime.AddMinutes((-1) * UserContext.TimeZoneOffset).FormatDateTime());

			return new MvcHtmlString(text + builder.ToString(TagRenderMode.Normal));
		}
	}
}

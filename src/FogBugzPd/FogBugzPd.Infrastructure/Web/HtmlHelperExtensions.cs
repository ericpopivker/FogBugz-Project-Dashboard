using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FogBugzPd.Infrastructure.Web
{
	public static class HtmlHelperExtensions
	{
		public static HtmlString PageNotification(this HtmlHelper htmlHelper, string message, PageNotificationType type, bool showIcon = false)
		{
			StringBuilder stringBuilder = new StringBuilder();

			//stringBuilder.Append("<div class=\"ui-widget\">");

			var template =
				"<div class=\"alert {0}\">{2}<a class=\"close\" onclick=\"fogbugzpd.web.closeAlert();return false;\" href=\"#\">×</a><p>{1}</p></div>";
			var result = "";
			//string style = "";
			//string icon = "";
			//string title = "";
			//string color = "";

			switch (type)
			{
				case PageNotificationType.Error:
					result = String.Format(template, "alert-error", message,
										  "");
					//style = "<div class=\"ui-state-error ui-corner-all\" style=\"padding: 0pt 07.em;\">";
					//icon = "<providern class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 0.3em;\"></providern>";
					//title = "<strong>Alert:</strong>&nbsp;";
					break;
				case PageNotificationType.Progress:
					result = String.Format(template, "alert-block", message, showIcon
											? "<span class=\"icon-progress\" style=\"float: left; margin-right: 0.3em;\"></span>"
											: "");
					//style = "<div class=\"ui-state-highlight  ui-corner-all\" style=\"padding: 0pt 07.em;\">";
					//icon = "<providern class=\"ui-icon-progress\" style=\"float: left; margin-right: 0.3em;\"></providern>";
					//title = "<strong style=\"color:#000000;\">Progress:</strong>&nbsp;";
					//color = "color:#000000;";
					break;
				case PageNotificationType.Success:
					result = String.Format(template, "alert-success", message, "");
					//style = "<div class=\"ui-state-highlight  ui-corner-all\" style=\"padding: 0pt 07.em;\">";
					//icon = "<providern class=\"ui-icon ui-icon-info\" style=\"float: left; margin-right: 0.3em;\"></providern>";
					//title = "";
					break;
				default:
					result = String.Format(template, "alert-info", message, "");
					//style = "<div class=\"ui-state-highlight  ui-corner-all\" style=\"padding: 0pt 07.em;\">";
					//icon = "<providern class=\"ui-icon ui-icon-info\" style=\"float: left; margin-right: 0.3em;\"></providern>";
					//title = "";
					break;
			}

			//stringBuilder.Append(style);
			//stringBuilder.Append("<p style=\"margin-top:0px; margin-bottom:0px; " + color + "\">");
			//stringBuilder.Append(icon);
			//stringBuilder.Append(title);
			//stringBuilder.Append(message);
			//stringBuilder.Append("</p>");
			//stringBuilder.Append("</div>");
			//stringBuilder.Append("</div>");
			stringBuilder.Append(result);

			return stringBuilder.ToHtmlString();
		}

		public static HtmlString ToHtmlString(this object obj)
		{
			if (obj == null) return new HtmlString("");

			return new HtmlString(obj.ToString());
		}

		public static string GetStatusClass(this decimal? value)
		{
			if (value == null) return "";

			return value.Value.GetStatusClass();
		}

		public static string GetStatusClass(this decimal value)
		{
			if (value < -3) return "verybehind";

			if (value <= 0) return "littlebehind";

			if (value <= 3) return "ontime";

			if (value <= 5) return "littleahead";

			return "veryahead";
		}
	}
}

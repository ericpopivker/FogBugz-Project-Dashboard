using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Web.Utils
{
	public static class HtmlGenerationUtils
	{
		public static HtmlString GenerateHolidaysTable(this IEnumerable<TimeOffRange> holidays)
		{
			var builder = new StringBuilder();

			builder.Append(@"<table class=""table"">");

			foreach (var holiday in holidays)
			{
				builder.Append("<tr>");

				builder.Append("<td>");

				builder.Append(HttpUtility.HtmlEncode(holiday.FromDate.FormatShortDate()).Replace(" ", "&nbsp;"));

				builder.Append("</td>");

				builder.Append("<td>");

				builder.Append(HttpUtility.HtmlEncode(holiday.ToDate.FormatShortDate()).Replace(" ", "&nbsp;"));

				builder.Append("</td>");

				builder.Append("<td>");

				builder.Append(HttpUtility.HtmlEncode(holiday.Name).Replace(" ", "&nbsp;"));

				builder.Append("</td>");

				builder.Append("</tr>");
			}

			builder.Append("</table>");

			return MvcHtmlString.Create(builder.ToString());
		}

		public static HtmlString GenerateAdjustedStatusCalculation(this EmployeeStatus employeeStatus, int daysToRelease)
		{
			var builder = new StringBuilder();

			builder.Append(employeeStatus.AdjustedOverUnder < 0
				               ? string.Format("{0} / {1}", employeeStatus.OverUnder, employeeStatus.PercentageWorkedDays/100)
				               : string.Format("{0} * {1}", employeeStatus.OverUnder, employeeStatus.PercentageWorkedDays/100));

			return MvcHtmlString.Create(builder.ToString());
		}

		public static HtmlString GenerateActualStatusCalculation(this EmployeeStatus employeeStatus, int daysToRelease)
		{
			var builder = new StringBuilder();

			if (employeeStatus.AdjustedOverUnder < 0)
				builder.Append(string.Format("{0} * 2", "working days accounting for last 5 days"));
			else
				builder.Append(string.Format("Days to release ({0}) - Remaining days ({1})", daysToRelease, StringUtils.FormatNonSignDecimal(employeeStatus.RemainingDays, 0, 0)));

			return MvcHtmlString.Create(builder.ToString());
		}

		public static HtmlString GenerateWorkedDaysCalculation(this EmployeeStatus employeeStatus)
		{
			var builder = new StringBuilder();

			if (employeeStatus.WorkedDays == 0)
				builder.Append(string.Format("0 worked days - 50%"));
			else
				builder.Append(string.Format("{0} worked days (from {3})/ {1} business days (from {2})", employeeStatus.WorkedDays, employeeStatus.BusinessDays, StringUtils.FormatShortDate(employeeStatus.MilestoneStartDate), StringUtils.FormatShortDate(employeeStatus.FirstWorkedDay)));

			return MvcHtmlString.Create(builder.ToString());
		}
	}
}

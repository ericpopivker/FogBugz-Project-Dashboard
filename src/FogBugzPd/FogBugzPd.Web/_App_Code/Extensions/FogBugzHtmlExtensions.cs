using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Enums;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Web._App_Code.Extensions;
using FogLampz.Model;

namespace FogBugzPd.Web
{
	public static class FogBugzHtmlExtensions
	{
		public static IHtmlString FogBugzCasesLink(this HtmlHelper helper, int number, FogBugzCasesLinkParams parameters, object attributes = null)
		{
			if (number == 0) return helper.Raw("0");

			return helper.FogBugzCasesLink(number.ToString(), parameters, attributes);
		}

		public static IHtmlString FogBugzCasesLink(this HtmlHelper helper, string caption, FogBugzCasesLinkParams parameters, object attributes = null)
		{
			var queryParts = new List<string>();

			var parametersStatus = String.Empty;
			if (parameters.Status.HasValue)
				parametersStatus = parameters.Status.ToString();

			Status status = FogBugzGateway.GetStatuses().FirstOrDefault(s => s.Index == parameters.StatusId || s.Name == parametersStatus);
			if (status == null && !String.IsNullOrEmpty(parametersStatus))
			{
				status = new Status {Name = parametersStatus};
			}

			if (parameters.PersonName == Person.ClosedUser.Name)
			{
				// if closed user
				if (parameters.Status.HasValue && parameters.Status.Value == CaseStatus.Closed || !parameters.Status.HasValue)
				{
					queryParts.Add(@"Status:""Closed""");
				}
			}
			else
			{
				// if any other user, including no user specified
				if (!string.IsNullOrEmpty(parameters.PersonName)) queryParts.Add(string.Format(@"AssignedTo:""{0}""", parameters.PersonName));

				if (parameters.StatusId.HasValue && status != null || status != null)
				{
					var statusQuery = String.Format(@"Status:""{0}""", status.Name);
					queryParts.Add(statusQuery);
				}
				if (status != null && status.Name == "Closed" || parametersStatus == "Closed")
				{
					queryParts.Add(@"Status:""Closed""");
				}
			}

			if (parameters.HasEstimate.HasValue)
			{
				if (parameters.HasEstimate == true)
				{
					queryParts.Add(@"EstimateCurrent:""*""");
				}
				else if (parameters.HasEstimate == false)
				{
					queryParts.Add(@"-EstimateCurrent:""*""");
				}
			}

			if (parameters.PriorityId.HasValue) queryParts.Add(string.Format(@"Priority:""{0}""", parameters.PriorityId.Value));

			if (!String.IsNullOrEmpty(parameters.Tag))
			{
				queryParts.Add(String.Format(@"Tag:""{0}""", parameters.Tag));
			}

			var query = string.Join(" ", queryParts);

			return helper.FogBugzCasesLink(caption, parameters.ProjectId, parameters.MilestoneId, parameters.SubProjectParentCaseId, query, attributes, parameters.HasChildren);
		}

		public static IHtmlString FogBugzCasesLink(this HtmlHelper helper, string caption, int? projectId, int? milestoneId, int? subProjectParentCaseId, string fogBugzQuery, object attributes = null, bool? hasChildren = null)
		{
			var uri = new UriBuilder(UserContext.FogBugzUrl) { Path = "default.asp" };

			var queryParts = new List<string>();

			if (projectId.HasValue)
				queryParts.Add(FogBugzGateway.GetProjectQueryPart(projectId.Value));

			if (milestoneId.HasValue)
				queryParts.Add(FogBugzGateway.GetMilestoneQueryPart(milestoneId.Value));

			if (subProjectParentCaseId.HasValue)
				queryParts.Add(FogBugzGateway.GetSubProjectQueryPart(subProjectParentCaseId.Value));


			if (!string.IsNullOrEmpty(fogBugzQuery)) queryParts.Add(CleanQuery(fogBugzQuery));

			//queryParts.Add(@"View:""Outline""");

			var query = string.Join(" ", queryParts);

			var nvc = new NameValueCollection
				{
					{"fNoChildren", "1"},
					{"sSearchFor", query},
					{"pre", "preSaveFilter"},
					{"fOpenBugs", "ON"},
					{"fClosedBugs", "ON"}
				};

			uri.Query = nvc.ToQueryString();

			return helper.Link(caption, uri.ToString(), HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
		}

		public static IHtmlString FogBugzCasesLink(this HtmlHelper helper, string caption, int? caseId, int? milestoneId, object attributes = null)
		{
			var uri = new UriBuilder(UserContext.FogBugzUrl) { Path = "default.asp" };

			var queryParts = new List<string>();

			queryParts.Add(String.Format("case:\"{0}\"", caseId));

			var milestone = milestoneId.HasValue ? FogBugzGateway.GetMilestone(milestoneId.Value) : null;
			if (milestone != null)
				queryParts.Add(FogBugzGateway.GetMilestoneQueryPart(milestone));

			var query = string.Join(" ", queryParts);

			var nvc = new NameValueCollection
				{
					{"sSearchFor", query},
					{"fOpenBugs", "ON"},
					{"fClosedBugs", "ON"}
				};

			uri.Query = nvc.ToQueryString();

			return helper.Link(caption, uri.ToString(), HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
		}

		private static string CleanQuery(string query)
		{
			return query.Replace(@"AssignedTo:""*Closed""", string.Empty);
		}

		private static string ToQueryString(this NameValueCollection nvc)
		{
			return string.Join("&", nvc.AllKeys.Select(key => string.Format(
				"{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
		}
	}
}

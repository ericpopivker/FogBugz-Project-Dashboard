using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Controllers;
using FogLampz.Model;

namespace FogBugzPd.Web._App_Code.Utils
{
	public class MilestoneUtils
	{
		public static DateTime? CodeFreezeCalculate(int projectId, Milestone milestone, int? projectParentCaseId, string name, Regex regEx)
		{
			if (DateUtils.GetCodeFreezeFromName(name, regEx) != null)
			{
				return DateUtils.GetCodeFreezeFromName(name, regEx);
			}
			if (milestone.DateStart == null || milestone.DateRelease == null || projectParentCaseId !=null)
			{
				return null;
			}
			DateTime codeFreeze;
			var dateReleaseMinusStart = (TimeSpan)(milestone.DateRelease - milestone.DateStart);
			var percentage = 1.0 - ((double)FbAccountContext.Current.Settings.QaPercentage / 100);
			if (!FbAccountContext.Current.Settings.AllowQaEstimates)
			{
				var calculatedTimeDifference = new TimeSpan(
					(int)(dateReleaseMinusStart.Days * percentage),
					(int)(dateReleaseMinusStart.Hours * percentage),
					(int)(dateReleaseMinusStart.Minutes * percentage),
					(int)(dateReleaseMinusStart.Seconds * percentage));
				codeFreeze = (DateTime)(milestone.DateStart + calculatedTimeDifference);
			}
			else
			{
				IList<Case> cases = FogBugzGateway.GetCases(projectId, milestone.Id);
				int totalQaEstimate = cases.Sum(@case => @case.TestEstimate.GetValueOrDefault(0));

				var timeSpan = new TimeSpan(0, 0, (int)totalQaEstimate, 0);
				var calculatedTimeDifference = dateReleaseMinusStart - timeSpan;
				codeFreeze = (DateTime)(milestone.DateStart + calculatedTimeDifference);
			}
			return codeFreeze;
		}

		public static bool CheckNotStarted(int? projectId, int? milestoneid, int? projectParentCaseId)
		{
			//true if status != NotStarted
			//false if status == NotStarted
			object storedObect = null;

			var projectStatusListItem = new ProjectStatusListItem()
			{
				ProjectId = projectId.Value,
				MileStoneId = milestoneid.Value,
				SubProjectParentCaseId = projectParentCaseId
			};

			var cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectsStatuses, UserContext.FogBugzUrl);

			Dictionary<ProjectStatusListItem, ProjectStatusEx> result = null;

			if (MsCache.TryGet(cacheKey, ref storedObect))
				result = (Dictionary<ProjectStatusListItem, ProjectStatusEx>) storedObect;
			else
			{
				var sessionKey = FbAccountContext.Current.Url + "ProjectStatusLoader_" + projectStatusListItem.Key;
				object storedStatusObject = null;

				if (MsCache.TryGet(sessionKey, ref storedStatusObject))
				{
					var status = (ProjectStatusEx)storedStatusObject;

					var ids = projectStatusListItem.Key.Split('_');
					var projectMilesToneItem = new ProjectStatusListItem();
					projectMilesToneItem.ProjectId = Convert.ToInt32(ids[0]);
					projectMilesToneItem.MileStoneId = Convert.ToInt32(ids[1]);
					if (ids.Count() > 2)
						projectMilesToneItem.SubProjectParentCaseId = Convert.ToInt32(ids[2]);
					result = new Dictionary<ProjectStatusListItem, ProjectStatusEx> {{projectMilesToneItem, status}};
				}
			}

			if (result != null)
			{
				//var listStatuses = result.ToList();

				var projectStatus = result.SingleOrDefault(ps => ps.Key.Key == projectStatusListItem.Key);
				
				if (projectStatus.Key != null)
				{
					return projectStatus.Value.Status != FogbugzProjectStatus.NotStarted;
				}

				//string resultStatus = String.Empty;

				//foreach (var status in listStatuses)
				//{
				//	if (projectParentCaseId == null)
				//	{
				//		if (status.Key.MileStoneId == milestoneid && status.Key.ProjectId == projectId)
				//		{
				//			resultStatus = status.Value.ToString();
				//		}
				//	}
				//	else if (status.Key.MileStoneId == milestoneid && status.Key.ProjectId == projectId &&
				//			 status.Key.SubProjectParentCaseId == projectParentCaseId)
				//	{
				//		resultStatus = status.Value.ToString();
				//	}
				//}

				//switch (resultStatus)
				//{
				//	case "NotStarted":
				//		return false;
				//	default:
				//		return true;
				//}
			}
			return true;
		}
	}
}
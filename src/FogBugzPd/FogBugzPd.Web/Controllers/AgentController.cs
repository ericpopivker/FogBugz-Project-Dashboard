using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using FogBugzPd.Application.Emails;
using FogBugzPd.Application.Extensions;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;
using FogLampz;
using FogLampz.Model;

namespace FogBugzPd.Web.Controllers
{
	public class AgentController : ControllerBase
	{
		public ActionResult SendDailyDigestEmails(string guid)
		{
			HttpContext.Server.ScriptTimeout = 600;

			Verify.Argument.IsNotEmpty(guid, "guid");

			var accountGuid = Guid.Parse(guid);
			
			var fbAccount = DbContext.FbAccounts
									 .Include("Settings")
									 .SingleOrDefault(a => a.Settings.Guid == accountGuid);

			if (fbAccount != null)
			{

				object projects = null;
				object milestones = null;
				object subProjectParentCases = null;

				//Getting projects and milestones
				var cacheKey = MsCacheKey.Gen(MsCacheDataType.TR_Projects, fbAccount.Url);

				MsCache.TryGet(cacheKey, ref projects);

				cacheKey = MsCacheKey.Gen(MsCacheDataType.TR_Milestones, fbAccount.Url);

				MsCache.TryGet(cacheKey, ref milestones);

				if (projects == null || milestones == null)
				{
					var client = new FogBugzClientEx();
					client.LogOn(fbAccount.Url + "/api.asp", fbAccount.Token);

					XCallContextUtils.SetData("FogBugzClient", client);

					UserContext.FbAccountId = fbAccount.Id;
					UserContext.IsLoggedIn = true;

					FbAccountContext.GetContextFbAccountIdMethod();

					projects = FogBugzGateway.GetProjects();
					milestones = FogBugzGateway.GetMilestones();
					subProjectParentCases = FogBugzGateway.GetSubProjectParentCases();
				}

				var projectMilestoneList = new ProjectMilestoneList(UserContext.FogBugzUrl);

				projectMilestoneList.GetList((List<Project>)projects, (List<FixFor>)milestones, (List<Case>)subProjectParentCases, 20);

				//getting statuses
				object storedObect = null;

				cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectsStatuses, fbAccount.Url);
				Dictionary<ProjectStatusListItem, ProjectStatus> result = null;

				if (MsCache.TryGet(cacheKey, ref storedObect))
				{
					var statuses = (Dictionary<ProjectStatusListItem, ProjectStatusEx>) storedObect;

					result = statuses.Select(s => new KeyValuePair<ProjectStatusListItem, ProjectStatus>(s.Key, new ProjectStatus(){Status = s.Value.Status, StatusValue = s.Value.StatusValue}))
							.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value); ;
				}
				else
				{
					var projectStatusesLoader = new ProjectStatusListLoader(fbAccount.Url);

					result = projectStatusesLoader.GetProjectStatuses(projectMilestoneList.Items);
				}

				//sending emails
				var emails = fbAccount.Settings.SendDailyDigestEmailsTo.Split(',');
				foreach (var email in emails)
				{
					var dailyDigest = new DailyDigest(projectMilestoneList, result, fbAccount.Settings.AllowSubProjects, email);
					dailyDigest.Send();
				}
			}
			return View();
		}
	}
}

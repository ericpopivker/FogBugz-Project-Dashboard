using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Application.Agent;
using FogBugzPd.Core;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Models.Project;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Web.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/
		[CustomAuthorize]
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Project");
		}

		public ActionResult Environment()
		{
			return View();
		}

		public ActionResult ClearCache()
		{
			MsCache.Clear();
			return RedirectToAction("Environment");
		}

		public ActionResult TestException()
		{
			throw new InvalidOperationException("Test 123");
		}

		public ActionResult Error(int? code)
		{
			return View("Error",code);
		}

		public ActionResult TestEmail()
		{
			var to = new List<string>();
			to.Add(EnvironmentConfig.GetDebugEmail());
			
			XEmailUtils.SendEmailsWithSendGrid("no-reply@fogbugzpd.com", to, "Test email", "Test email from FogbugzPD", true, null);
			
			return RedirectToAction("Environment");
		}

		[HttpPost]
		public JsonResult ClearMsCache(int type, int? projectId, int? milestoneId, int? subProjectParentCaseId)
		{
			var msCacheDataType = (MsCacheDataType) type;
			string cacheKey;

			switch (msCacheDataType)
			{
					case MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl:
						cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, UserContext.FogBugzUrl);
						MsCache.TryRemove(cacheKey);
						break;
					case MsCacheDataType.FogBugz_CaseSet:
						cacheKey = MsCacheKey.GenCaseSetKey(projectId.Value, milestoneId.Value,subProjectParentCaseId);
						MsCache.TryRemove(cacheKey);
						break;
					case MsCacheDataType.ProjectLists:
						cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectLists, UserContext.FogBugzUrl);
						MsCache.TryRemove(cacheKey);
						break;
					default:
						throw new ArgumentOutOfRangeException("msCacheDataType");

			}
			
			return Json(0);
		}

		public ActionResult RunDailyDigestJob()
		{
			HttpContext.Server.ScriptTimeout = 600;
			
			AgentUtils.RunJobByName("DailyDigestEmailJob");

			return RedirectToAction("Environment");
		}
	}
}

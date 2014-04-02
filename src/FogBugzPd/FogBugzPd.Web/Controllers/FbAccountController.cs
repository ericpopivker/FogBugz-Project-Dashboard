using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FogBugzPd.Core;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Models.FbAccount;
using FogBugzPd.Application;
using System.Linq;
using FogBugzPd.Infrastructure.Web;
using FogLampz;
using FogLampz.Model;

namespace FogBugzPd.Web.Controllers
{
    public class FbAccountController : ControllerBase
    {
		[HttpGet]
		[CustomAuthorize]
		public ActionResult Settings()
		{
			var fbAccount = DbContext.FbAccounts
			                          .Include("Settings")
			                          .Include("Settings.TestRailConfig")
			                          .Single(a => a.Id == UserContext.FbAccountId);
			FbAccountSettings fbAccountSettings;

			if (fbAccount == null)
				fbAccountSettings = new FbAccountSettings();
			else
				fbAccountSettings = fbAccount.Settings;

			var viewModel = new SettingViewModel();
	
			viewModel.Setup();

			MapSettingsViewModel(viewModel, fbAccountSettings);

			return View("Update", viewModel);
        }

	    [HttpPost]
		[CustomAuthorize]
		public ActionResult Settings(SettingViewModel viewModel)
	    {
			viewModel.Setup();

			if (!ModelState.IsValid)
			{
				return View("Update", viewModel);
			}

			var fbAccount = DbContext.FbAccounts
									.Include("Settings")
									.Include("Settings.TestRailConfig")
									.SingleOrDefault(s => s.Id == viewModel.Id);

			FbAccountSettings fbAccountSettings;

			if (fbAccount == null)
				fbAccountSettings = new FbAccountSettings();
			else
				fbAccountSettings = fbAccount.Settings;

			//refresh cache when qa estimate settings changed
			if (fbAccountSettings.AllowQaEstimates != viewModel.AllowQaEstimates
				|| fbAccountSettings.QaEstimateCustomFieldName != viewModel.QaEstimateCustomFieldname
				|| fbAccountSettings.QaPercentage != viewModel.QaPercentage)
		    {
				CleareCachedCaseSets();
		    }

			MapSettingsEntity(fbAccountSettings, viewModel);

		    var service = new FbAccountService();

		    if (fbAccount != null && fbAccount.Settings != null)
		    {
			    if (fbAccount.Settings.SendDailyDigestEmails)
			    {
					string cacheKeyToken = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, UserContext.FogBugzUrl);
				    object fbClientObject = null;
					MsCache.TryGet(cacheKeyToken, ref fbClientObject);
				    var fbClient = (FogBugzClientEx) fbClientObject;
				    fbAccount.Token = fbClient.GetToken();
			    }
			    else
			    {
				    fbAccount.Token = String.Empty;
			    }
		    }

		    // save settings in db
			service.Update(fbAccount);

			var cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectLists, UserContext.FogBugzUrl);
			MsCache.TryRemove(cacheKey);

			cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, UserContext.FogBugzUrl);
			MsCache.TryRemove(cacheKey);

			UserContext.SetNotification(PageNotificationType.Success, "Settings have been updated.");

			return RedirectToAction("Settings");
	    }

	    private void MapSettingsViewModel(SettingViewModel viewModel, FbAccountSettings fbAccountSettings)
	    {
			viewModel.Id = fbAccountSettings.Id;
		    viewModel.ResolvedVerifiedStatusId = fbAccountSettings.ResolvedVerifiedStatusId;
			viewModel.AllowSubProjects = fbAccountSettings.AllowSubProjects;
			viewModel.QaPercentage = fbAccountSettings.QaPercentage;
			viewModel.AllowTestRail = fbAccountSettings.AllowTestRail;
			viewModel.AllowSendDailyDigestEmails = fbAccountSettings.SendDailyDigestEmails;

			if (fbAccountSettings.SendDailyDigestEmails)
			{
				viewModel.SendDailyDigestEmailsTo = fbAccountSettings.SendDailyDigestEmailsTo;
			}

			if (fbAccountSettings.TestRailConfig != null)
			{
				viewModel.TestRailUrl = fbAccountSettings.TestRailConfig.Url;
				viewModel.TestRailToken = fbAccountSettings.TestRailConfig.Token;
			}
		    
			viewModel.SubProjectTag = fbAccountSettings.SubProjectTag;

		    viewModel.AllowQaEstimates = fbAccountSettings.AllowQaEstimates;
		    viewModel.QaEstimateCustomFieldname = fbAccountSettings.QaEstimateCustomFieldName;
	    }

		private void MapSettingsEntity(FbAccountSettings entity, SettingViewModel viewModel)
		{
			entity.Id = viewModel.Id;
			entity.ResolvedVerifiedStatusId = viewModel.ResolvedVerifiedStatusId;
			entity.AllowSubProjects = viewModel.AllowSubProjects;
			entity.QaPercentage = viewModel.QaPercentage ?? 0;
			entity.AllowTestRail = viewModel.AllowTestRail;
			entity.SendDailyDigestEmails = viewModel.AllowSendDailyDigestEmails;

			if (entity.SendDailyDigestEmails)
			{
				entity.SendDailyDigestEmailsTo = viewModel.SendDailyDigestEmailsTo;
				entity.Guid = Guid.NewGuid();
			}
			else
			{
				entity.SendDailyDigestEmailsTo = String.Empty;
				entity.Guid = null;
			}

			if (entity.AllowTestRail)
			{
				if(entity.TestRailConfig == null)
					entity.TestRailConfig = new TestRailConfiguration();


				entity.TestRailConfig.Url = viewModel.TestRailUrl;
				entity.TestRailConfig.Token = viewModel.TestRailToken;
			}
			entity.SubProjectTag = viewModel.SubProjectTag;

			entity.AllowQaEstimates = viewModel.AllowQaEstimates;
			entity.QaEstimateCustomFieldName = viewModel.QaEstimateCustomFieldname;
		}

		private void CleareCachedCaseSets()
		{
			IList<FixFor> milestones = FogBugzGateway.GetMilestones();

			milestones.ToList().ForEach(
				milestone =>
				{
					if (milestone.IndexProject.HasValue)
					{
						string cacheKey = MsCacheKey.GenCaseSetKey(milestone.IndexProject.Value, milestone.Index.Value);
						MsCache.TryRemove(cacheKey);
					}
				}
				);


			IList<Case> subProjectParentCases = FogBugzGateway.GetSubProjectParentCases();

			if (subProjectParentCases != null)
				subProjectParentCases.ToList().ForEach(
					parentCase =>
					{
						string cacheKey = MsCacheKey.GenCaseSetKey(parentCase.IndexProject.Value, parentCase.IndexFixFor.Value,
																   parentCase.Index);
						MsCache.TryRemove(cacheKey);
					}
					);

		}
    }
}

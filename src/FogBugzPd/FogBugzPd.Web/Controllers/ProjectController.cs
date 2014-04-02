using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Application.Extensions;
using FogBugzPd.Core;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.ProgressStatus;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Models.Project;
using FogBugzPd.Web.Utils;
using FogBugzPd.Web._App_Code.Utils;
using FogLampz;
using FogLampz.Model;

namespace FogBugzPd.Web.Controllers
{
	public class ProjectController : ControllerBase
	{
		//coding convention from http://stackoverflow.com/questions/242534/c-sharp-naming-convention-for-constants

		private static readonly Regex CodeFreezeSubstringRegex = new Regex(
			@"\(CF *(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2}(?:\d{2})?)\)",
			RegexOptions.Singleline | RegexOptions.Compiled);

		private const string ProjectStatusKeyPreffix = "ProjectStatusLoader";

		private const string ProjectStatusesKey = "AllKeys";

		private const int DefaultNumOfThreads = 20;

		private const string ListProgressStatusKey = "ProjectListStatusKey";

		[CustomAuthorize]
		public ActionResult Index()
		{
			return RedirectToAction("List");
		}

		[CustomAuthorize]
		public ActionResult List()
		{
			MsCache.Set(ListProgressStatusKey + "_" + UserContext.FogBugzUrl, new ProgressStatusInfo { Value = 0, Label = " Getting projects and milestones..." });

			ViewBag.MasterTitle = "Projects and Milestones";

			return View("List");
		}


		[CustomAuthorize]
		public ActionResult AsyncList(int timeOffest)
		{
			UserContext.TimeZoneOffset = timeOffest;
			var model = CreateListViewModel();

			model.ShowSubProjects = FbAccountContext.Current.Settings.AllowSubProjects;

			var cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectsStatuses, UserContext.FogBugzUrl);

			object storedObect = null;

			if (!MsCache.TryGet(cacheKey, ref storedObect))
			{

				var projectStatusesLoader = new ProjectStatusListLoader(FbAccountContext.Current.Url);
				projectStatusesLoader.SaveStatusEvent += OnSaveStatus;

				var keys = projectStatusesLoader.BeginGettingProjectStatuses(model.Items);
				MsCache.Set(FbAccountContext.Current.Url + ProjectStatusesKey, keys);
			}

			return PartialView("_AsyncList", model);
		}

		[CustomAuthorize]
		public ActionResult Dashboard(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			if (!MilestoneUtils.CheckNotStarted(projectId, milestoneId, subProjectParentCaseId))
				return RedirectToRoute("DashboardNotStarted");

			var viewModel = new DashboardViewModel();
			viewModel.Setup(projectId, milestoneId, subProjectParentCaseId, true);

			UpdateViewBagForProjectLayout(viewModel);

			return View(viewModel);
		}

		[CustomAuthorize]
		public ActionResult AsyncDashboard(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			var today = DateTime.Today;

			var model = new AsyncDashboardViewModel();
			model.Setup(projectId, milestoneId, subProjectParentCaseId);

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			UpdateViewBagForProjectLayout(model);

			IList<Case> cases = new List<Case>();

			Case projectParentCase = null;
			if (subProjectParentCaseId.HasValue)
			{
				var client = FogBugzGateway.GetClientForParallel();
				projectParentCase = client.GetCase(subProjectParentCaseId.Value);
			}

			foreach (var casesWithMilestone in model.CaseSet.Cases.Where(casesWithMilestone => casesWithMilestone.IndexFixFor == milestoneId))
			{
				cases.Add(casesWithMilestone);
			}

			var fbSchedule = FogBugzGateway.GetFbSchedule();

			if (model.CaseSet.Milestone != null)
			{
				model.DatesSection.StartDate = model.CaseSet.Milestone.DateStart;
				if (model.DatesSection.StartDate.HasValue)
				{
					model.DatesSection.StartDateDaysRemaining = DateUtils.Difference(today, model.DatesSection.StartDate.Value);
				}


				model.DatesSection.CodeFreeze = (projectParentCase != null && projectParentCase.DateDue != null) ? projectParentCase.DateDue : MilestoneUtils.CodeFreezeCalculate(projectId, model.Milestone, model.SubProjectParentCaseId, model.CaseSet.Milestone.Name, CodeFreezeSubstringRegex);

				if (model.DatesSection.CodeFreeze.HasValue)
				{
					model.DatesSection.CodeFreezeDaysRemaining = DateUtils.Difference(today, model.DatesSection.CodeFreeze.Value);
					//model.DatesSection.CodeFreezeHolidaysBefore = HolidaysUtils.GetHolidaysInRange(today, model.DatesSection.CodeFreeze.Value);

					model.DatesSection.CodeFreezeHolidaysBefore = fbSchedule.GetSiteScheduledDays(today, model.DatesSection.CodeFreeze.Value).TimeOffRanges;
				}

				model.DatesSection.Rollout = projectParentCase != null ? (model.CaseSet.Milestone.Name.Contains("CF") ? DateUtils.GetCodeFreezeFromName(model.CaseSet.Milestone.Name, CodeFreezeSubstringRegex) : null) : model.CaseSet.Milestone.DateRelease;
				if (model.DatesSection.Rollout.HasValue)
				{
					model.DatesSection.RolloutDaysRemaining = DateUtils.Difference(today, model.DatesSection.Rollout.Value);
					//model.DatesSection.RolloutHolidaysBefore = HolidaysUtils.GetHolidaysInRange(today, model.DatesSection.Rollout.Value);
					model.DatesSection.RolloutHolidaysBefore = fbSchedule.GetSiteScheduledDays(today, model.DatesSection.Rollout.Value).TimeOffRanges;
				}

				CalculatePlotTimes(model.DatesSection);

				model.DatesSection.IsActiveProject = today >= model.DatesSection.StartDate && today <= model.DatesSection.Rollout;
			}

			var persons = FogBugzGateway.GetPersons();
			var isLoadedByAdmin = FogBugzGateway.GetIsScheduleLoadedByAdmin();

			var projectStatusCalculator = new ProjectStatusCalculator(model.CaseSet, persons.ToList(), isLoadedByAdmin, fbSchedule);
			model.ProjectStatus = projectStatusCalculator.GetProjectStatus();

			return PartialView("_AsyncDashboard", model);
		}

		private static void CalculatePlotTimes(AsyncDashboardViewModel.DatesSectionViewModel datesTab)
		{
			var plotTimes = new List<AsyncDashboardViewModel.DatesSectionViewModel.PlotTime>();

			var dtCurrent = DateTime.Now;

			// 0. Start time
			if (datesTab.StartDate.HasValue) plotTimes.Add(new AsyncDashboardViewModel.DatesSectionViewModel.PlotTime("Start Date", datesTab.StartDate.Value));

			// 1. Current time
			plotTimes.Add(new AsyncDashboardViewModel.DatesSectionViewModel.PlotTime("Now", dtCurrent));

			if (datesTab.StartDate.HasValue)
			{
				datesTab.MinimumTime = TimeUtils.Min(dtCurrent, datesTab.StartDate.Value);
			}

			// 2. Code freeze time
			var codeFreeze = datesTab.CodeFreeze;
			if (codeFreeze.HasValue) plotTimes.Add(new AsyncDashboardViewModel.DatesSectionViewModel.PlotTime("Code Freeze", codeFreeze.Value));

			datesTab.MinimumTime = (datesTab.StartDate.HasValue
										? TimeUtils.Min(dtCurrent, datesTab.StartDate.Value)
										: (codeFreeze.HasValue
											   ? TimeUtils.Min(dtCurrent, codeFreeze.Value)
											   : dtCurrent)).AddDays(-7);

			// 3. Release date
			if (datesTab.Rollout != null)
			{
				plotTimes.Add(new AsyncDashboardViewModel.DatesSectionViewModel.PlotTime("Release", datesTab.Rollout.Value));

				datesTab.MaximumTime = TimeUtils.Max(dtCurrent, datesTab.Rollout.Value).AddDays(7);

				datesTab.PlotTimes = plotTimes.OrderBy(plotTime => plotTime.DateTime).ToArray();
			}


		}

		//private static DateTime? GetCodeFreezeFromName(string name)
		//{
		//	Match match = CodeFreezeSubstringRegex.Match(name);

		//	int day, month, year;

		//	if (match.Success &&
		//		int.TryParse(match.Groups["day"].Value, out day) &&
		//		int.TryParse(match.Groups["month"].Value, out month) &&
		//		int.TryParse(match.Groups["year"].Value, out year))
		//	{
		//		if (year < 100) year += 2000;

		//		return new DateTime(year, month, day);
		//	}

		//	return null;
		//}

		[CustomAuthorize]
		public ActionResult Priority(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			var model = new PriorityViewModel();

			model.Setup(projectId, milestoneId, subProjectParentCaseId);

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			UpdateViewBagForProjectLayout(model);

			return View(model);
		}

		[CustomAuthorize]
		public ActionResult Developer(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			var model = new DeveloperViewModel();

			model.Setup(projectId, milestoneId, subProjectParentCaseId);
			UpdateViewBagForProjectLayout(model);

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			return View(model);
		}

		[CustomAuthorize]
		public ActionResult QA(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			var model = new QAViewModel();

			model.Setup(projectId, milestoneId, subProjectParentCaseId);
			UpdateViewBagForProjectLayout(model);
			var persons = FogBugzGateway.GetPersons();

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			IList<Case> cases = new List<Case>();
			foreach (var casesWithMilestone in model.CaseSet.Cases.Where(casesWithMilestone => casesWithMilestone.IndexFixFor == milestoneId))
			{
				cases.Add(casesWithMilestone);
			}

			var qaListItems =
				persons.GroupJoin(cases, person => person.Index, @case => @case.IndexPersonAssignedTo,
								  (person, personCases) =>
								  {
									  var casesArr = personCases.ToArray();
									  return new QAListItem
										  {
											  TesterName = person.Name,
											  Estimate = casesArr.Where(CaseUtils.IsResolved).Sum(@case => CalcEstimateForCase(@case)),
											  CaseToVerify = casesArr.Count(CaseUtils.IsResolved),
											  DevelopmentTime = casesArr.Where(CaseUtils.IsResolved).Sum(@case => @case.HoursElapsed.GetValueOrDefault(0)),
											  VerifiedCases = casesArr.Count(CaseUtils.IsResolvedVerified)
										  };
								  })
					   .Where(li => li.CaseToVerify > 0 || li.VerifiedCases > 0);


			model.SetItems(qaListItems);

			model.CasesTotal = cases.Count();

			model.CasesReadyToBeTested = cases.Count(c => c.IndexStatus == FbAccountContext.Current.Settings.ResolvedVerifiedStatusId);
			model.CasesActive = cases.Count(CaseUtils.IsActive);
			model.CasesVerified = model.CaseSet.Cases.Count(CaseUtils.IsResolvedVerified);
			model.CasesClosed = cases.Count(CaseUtils.IsClosed);


			decimal activeCasesTestingTime =
				cases.Where(CaseUtils.IsActive).Sum(@case => CalcEstimateForCase(@case, true));
			decimal resolvedCasesTestingTime =
				cases.Where(CaseUtils.IsResolved).Sum(@case => CalcEstimateForCase(@case));
			decimal resolvedVerifiedCasesTestingTime =
				cases.Where(CaseUtils.IsResolvedVerified).Sum(@case => CalcEstimateForCase(@case));
			decimal closedCasesTestingTime =
				cases.Where(CaseUtils.IsClosed).Sum(@case => CalcEstimateForCase(@case));

			model.TotalTestingTime = (activeCasesTestingTime + resolvedCasesTestingTime + closedCasesTestingTime + resolvedVerifiedCasesTestingTime);
			model.RemainingTestingTime = (activeCasesTestingTime + resolvedCasesTestingTime);
			model.ReadyToBeTestedTime = resolvedCasesTestingTime;

			if (FbAccountContext.Current.Settings.AllowTestRail)
				model.TestRailPlansSummary = FogBugzToTestRailUtils.GetTestRailPlansSummary(milestoneId, projectId);

			return View(model);
		}

		private decimal CalcEstimateForCase(Case cs, bool currentEstimate = false)
		{
			decimal result = 0;

			var ratio = (decimal)FbAccountContext.Current.Settings.QaPercentage / 100;

			if (currentEstimate)
			{
				if (FbAccountContext.Current.Settings.AllowQaEstimates)
					result = ((decimal)cs.TestEstimate.GetValueOrDefault(0)) / 60m; // "Test Estimate" field value is in minutee
				else
					result = cs.HoursCurrentEstimate.GetValueOrDefault(0) * ratio;
			}
			else if (FbAccountContext.Current.Settings.AllowQaEstimates)
			{
				result = ((decimal)cs.TestEstimate.GetValueOrDefault(0))/60m; // "Test Estimate" field value is in minutes
			}
			else
			{
				result = cs.HoursElapsed.GetValueOrDefault(0) * ratio;
			}

			return result;
		}

		[CustomAuthorize]
		public ActionResult AdvancedDashboard(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			var viewModel = new AdvancedDashboardViewModel();
			viewModel.Setup(projectId, milestoneId, subProjectParentCaseId, true);

			UpdateViewBagForProjectLayout(viewModel);

			return View(viewModel);
		}

		[CustomAuthorize]
		public ActionResult AsyncAdvancedDashboard(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			var today = DateTime.Today;

			var model = new AsyncAdvancedDashboardViewModel();
			model.Setup(projectId, milestoneId, subProjectParentCaseId);

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			UpdateViewBagForProjectLayout(model);

			IList<Case> cases = new List<Case>();

			foreach (var casesWithMilestone in model.CaseSet.Cases.Where(casesWithMilestone => casesWithMilestone.IndexFixFor == milestoneId))
			{
				cases.Add(casesWithMilestone);
			}

			model.CasesSection.Total = cases.Count();
			model.CasesSection.Active = cases.Count(CaseUtils.IsActive);
			model.CasesSection.Resolved = cases.Count(CaseUtils.IsResolved);
			model.CasesSection.Verified = cases.Count(CaseUtils.IsResolvedVerified);
			model.CasesSection.Closed = cases.Count(CaseUtils.IsClosed);

			model.TimeSection.TotalEstimated = cases.Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.TimeSection.Elapsed = cases.Sum(@case => @case.HoursElapsed.GetValueOrDefault(0));
			model.TimeSection.ActiveEstimated = cases.Where(CaseUtils.IsActive).Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.TimeSection.ActiveRemaining = cases.Where(CaseUtils.IsActive).Sum(@case => Math.Max(
				0, @case.HoursCurrentEstimate.GetValueOrDefault(0) - @case.HoursElapsed.GetValueOrDefault(0)));

			model.EstimatesSection.WithEstimates = cases.Where(CaseUtils.IsActive).Count(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0) > 0);
			model.EstimatesSection.WithoutEstimates = cases.Count(CaseUtils.IsActive) - model.EstimatesSection.WithEstimates;

			//at this point model.EstimatesSection.WithoutEstimates can be negative.
			//it is possible when cases which has subcases (IndexBugChildren.Count>0) are estimated too
			if (model.EstimatesSection.WithoutEstimates < 0)
				model.EstimatesSection.WithoutEstimates = 0;
			model.EstimatesSection.GoingOverEstimate = cases.Where(CaseUtils.IsActive).Count(
				@case => @case.HoursElapsed.GetValueOrDefault(0) > @case.HoursCurrentEstimate.GetValueOrDefault(0));

			model.AccuracySection.EstimatedTime = cases.Where(@case => CaseUtils.IsResolved(@case) || CaseUtils.IsClosed(@case)).Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.AccuracySection.ActualTime = cases.Where(@case => CaseUtils.IsResolved(@case) || CaseUtils.IsClosed(@case)).Sum(@case => @case.HoursElapsed.GetValueOrDefault(0));

			return PartialView("_AsyncAdvancedDashboard", model);
		}

		#region DashboardNotStarted Section
		[CustomAuthorize]
		public ActionResult DashboardNotStarted(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			var viewModel = new DashboardNotStartedViewModel();
			viewModel.Setup(projectId, milestoneId, subProjectParentCaseId, true);

			UpdateViewBagForProjectLayout(viewModel);

			return View(viewModel);
		}

		[CustomAuthorize]
		public ActionResult AsyncDashboardNotStarted(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			var today = DateTime.Today;

			var model = new AsyncDashboardNotStartedViewModel();
			model.Setup(projectId, milestoneId, subProjectParentCaseId);

			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);
			model.MsCache.CreatedAt = MsCache.GetObjectExpirationTime(cacheKey);

			UpdateViewBagForProjectLayout(model);

			IList<Case> cases = new List<Case>();

			foreach (var casesWithMilestone in model.CaseSet.Cases.Where(casesWithMilestone => casesWithMilestone.IndexFixFor == milestoneId))
			{
				cases.Add(casesWithMilestone);
			}

			model.CasesSection.Total = cases.Count();
			model.CasesSection.Active = cases.Count(CaseUtils.IsActive);
			model.CasesSection.Resolved = cases.Count(CaseUtils.IsResolved);
			model.CasesSection.Verified = cases.Count(CaseUtils.IsResolvedVerified);
			model.CasesSection.Closed = cases.Count(CaseUtils.IsClosed);

			model.TimeSection.TotalEstimated = cases.Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.TimeSection.Elapsed = cases.Sum(@case => @case.HoursElapsed.GetValueOrDefault(0));
			model.TimeSection.ActiveEstimated = cases.Where(CaseUtils.IsActive).Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.TimeSection.ActiveRemaining = cases.Where(CaseUtils.IsActive).Sum(@case => Math.Max(
				0, @case.HoursCurrentEstimate.GetValueOrDefault(0) - @case.HoursElapsed.GetValueOrDefault(0)));

			model.EstimatesSection.WithEstimates = cases.Where(CaseUtils.IsActive).Count(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0) > 0);
			model.EstimatesSection.WithoutEstimates = cases.Count(CaseUtils.IsActive) - model.EstimatesSection.WithEstimates;

			//at this point model.EstimatesSection.WithoutEstimates can be negative.
			//it is possible when cases which has subcases (IndexBugChildren.Count>0) are estimated too
			if (model.EstimatesSection.WithoutEstimates < 0)
				model.EstimatesSection.WithoutEstimates = 0;
			model.EstimatesSection.GoingOverEstimate = cases.Where(CaseUtils.IsActive).Count(
				@case => @case.HoursElapsed.GetValueOrDefault(0) > @case.HoursCurrentEstimate.GetValueOrDefault(0));

			model.AccuracySection.EstimatedTime = cases.Where(@case => CaseUtils.IsResolved(@case) || CaseUtils.IsClosed(@case)).Sum(@case => @case.HoursCurrentEstimate.GetValueOrDefault(0));
			model.AccuracySection.ActualTime = cases.Where(@case => CaseUtils.IsResolved(@case) || CaseUtils.IsClosed(@case)).Sum(@case => @case.HoursElapsed.GetValueOrDefault(0));

			Case projectParentCase = null;
			if (subProjectParentCaseId.HasValue)
			{
				var client = FogBugzGateway.GetClientForParallel();
				projectParentCase = client.GetCase(subProjectParentCaseId.Value);
			}

			foreach (var casesWithMilestone in model.CaseSet.Cases.Where(casesWithMilestone => casesWithMilestone.IndexFixFor == milestoneId))
			{
				cases.Add(casesWithMilestone);
			}

			var fbSchedule = FogBugzGateway.GetFbSchedule();

			if (model.CaseSet.Milestone != null)
			{
				model.DatesSection.StartDate = model.CaseSet.Milestone.DateStart;
				if (model.DatesSection.StartDate.HasValue)
				{
					model.DatesSection.StartDateDaysRemaining = DateUtils.Difference(today, model.DatesSection.StartDate.Value);
				}


				model.DatesSection.CodeFreeze = (projectParentCase != null && projectParentCase.DateDue != null) ? projectParentCase.DateDue : MilestoneUtils.CodeFreezeCalculate(projectId, model.Milestone, model.SubProjectParentCaseId, model.CaseSet.Milestone.Name, CodeFreezeSubstringRegex);
				if (model.DatesSection.CodeFreeze.HasValue)
				{
					model.DatesSection.CodeFreezeDaysRemaining = DateUtils.Difference(today, model.DatesSection.CodeFreeze.Value);

					model.DatesSection.CodeFreezeHolidaysBefore = fbSchedule.GetSiteScheduledDays(today, model.DatesSection.CodeFreeze.Value).TimeOffRanges;
				}

				model.DatesSection.Rollout = projectParentCase != null ? (model.CaseSet.Milestone.Name.Contains("CF") ? DateUtils.GetCodeFreezeFromName(model.CaseSet.Milestone.Name, CodeFreezeSubstringRegex) : null) : model.CaseSet.Milestone.DateRelease;
				if (model.DatesSection.Rollout.HasValue)
				{
					model.DatesSection.RolloutDaysRemaining = DateUtils.Difference(today, model.DatesSection.Rollout.Value);
					model.DatesSection.RolloutHolidaysBefore = fbSchedule.GetSiteScheduledDays(today, model.DatesSection.Rollout.Value).TimeOffRanges;
				}

				CalculatePlotTimesNotStarted(model.DatesSection);

				model.DatesSection.IsActiveProject = today >= model.DatesSection.StartDate && today <= model.DatesSection.Rollout;
			}

			var persons = FogBugzGateway.GetPersons();
			var isLoadedByAdmin = FogBugzGateway.GetIsScheduleLoadedByAdmin();


			var projectStatusCalculator = new ProjectStatusCalculator(model.CaseSet, persons.ToList(), isLoadedByAdmin, fbSchedule);
			model.ProjectStatus = projectStatusCalculator.GetProjectStatus();

			return PartialView("_AsyncDashboardNotStarted", model);
		}

		private static void CalculatePlotTimesNotStarted(AsyncDashboardNotStartedViewModel.DatesSectionViewModel datesTab)
		{
			var plotTimes = new List<AsyncDashboardNotStartedViewModel.DatesSectionViewModel.PlotTime>();

			var dtCurrent = DateTime.Now;

			// 0. Start time
			if (datesTab.StartDate.HasValue) plotTimes.Add(new AsyncDashboardNotStartedViewModel.DatesSectionViewModel.PlotTime("Start Date", datesTab.StartDate.Value));

			// 1. Current time
			plotTimes.Add(new AsyncDashboardNotStartedViewModel.DatesSectionViewModel.PlotTime("Now", dtCurrent));

			if (datesTab.StartDate.HasValue)
			{
				datesTab.MinimumTime = TimeUtils.Min(dtCurrent, datesTab.StartDate.Value);
			}

			// 2. Code freeze time
			var codeFreeze = datesTab.CodeFreeze;
			if (codeFreeze.HasValue) plotTimes.Add(new AsyncDashboardNotStartedViewModel.DatesSectionViewModel.PlotTime("Code Freeze", codeFreeze.Value));

			datesTab.MinimumTime = (datesTab.StartDate.HasValue
										? TimeUtils.Min(dtCurrent, datesTab.StartDate.Value)
										: (codeFreeze.HasValue
											   ? TimeUtils.Min(dtCurrent, codeFreeze.Value)
											   : dtCurrent)).AddDays(-7);

			// 3. Release date
			if (datesTab.Rollout != null)
			{
				plotTimes.Add(new AsyncDashboardNotStartedViewModel.DatesSectionViewModel.PlotTime("Release", datesTab.Rollout.Value));

				datesTab.MaximumTime = TimeUtils.Max(dtCurrent, datesTab.Rollout.Value).AddDays(7);

				datesTab.PlotTimes = plotTimes.OrderBy(plotTime => plotTime.DateTime).ToArray();
			}
		}
		#endregion

		public JsonResult GetStatuses()
		{
			object storedObect = null;

			var cacheKey = MsCacheKey.Gen(MsCacheDataType.ProjectsStatuses, UserContext.FogBugzUrl);

			Dictionary<ProjectStatusListItem, ProjectStatusEx> result = null;

			var iscomplete = true;

			if (MsCache.TryGet(cacheKey, ref storedObect))
				result = (Dictionary<ProjectStatusListItem, ProjectStatusEx>)storedObect;
			else
				result = GetLatestProjectStatuses(ref iscomplete);

			var statuses = result.Select(d => new { Id = d.Key.Key.Replace("_", "-"), Status = GenerateStatusLabel(d.Value, d.Key).ToString() }).ToList();

			if (iscomplete && storedObect == null)
			{
				MsCache.Set(cacheKey, result);

				ClearProjectStatusesCacheData();
			}

			if (iscomplete) 
				MsCache.Set(ListProgressStatusKey + "_" + UserContext.FogBugzUrl, new ProgressStatusInfo { Value = 100, Label = "Complete" });

			return Json(new { iscomplete, Results = statuses });
		}

		public JsonResult GetProgressStatus()
		{
			object progressData = null;

			if (MsCache.TryGet(ListProgressStatusKey + "_" + UserContext.FogBugzUrl, ref progressData))
			{
				var progressStatus = progressData as ProgressStatusInfo;

				return Json(new { progressStatus.Value, progressStatus.Label });
			}

			return Json(new { Value = 100, Label = "" });
		}

		private ProjectMilestoneList CreateListViewModel()
		{
			IList<Project> projects = FogBugzGateway.GetProjects();
			IList<FixFor> milestones = FogBugzGateway.GetMilestones();
			IList<Case> subProjectParentCases = FogBugzGateway.GetSubProjectParentCases();

			var projectMilestoneList = new ProjectMilestoneList(UserContext.FogBugzUrl);

			projectMilestoneList.GetList(projects, milestones, subProjectParentCases, DefaultNumOfThreads);

			return projectMilestoneList;
		}

		private void UpdateViewBagForProjectLayout(CaseSetViewModelBase viewModel)
		{
			ViewBag.ProjectName = viewModel.ProjectName;
			ViewBag.MilestoneName = viewModel.MilestoneName;

			if (viewModel.SubProjectParentCaseId.HasValue)
			{
				var subProjectParentCase = FogBugzGateway.GetSubProjectParentCase(viewModel.SubProjectParentCaseId.Value);
				ViewBag.SubProjectName = subProjectParentCase.Title;
			}

			if (viewModel.MilestoneId.HasValue)
			{
				//var codeFreezeDate = DateUtils.GetCodeFreezeFromName(viewModel.MilestoneName, CodeFreezeSubstringRegex);
				var codeFreezeDate = MilestoneUtils.CodeFreezeCalculate((int)viewModel.ProjectId, viewModel.Milestone, viewModel.SubProjectParentCaseId, viewModel.MilestoneName, CodeFreezeSubstringRegex);
				var endDate = viewModel.Milestone.DateRelease;
				var startDate = viewModel.Milestone.DateStart;
				int unixTime = 0;
				if (startDate.HasValue && DateTime.Now >= startDate)
				{
					if (codeFreezeDate.HasValue && codeFreezeDate.Value > DateTime.Now)
					{
						unixTime = (int)(codeFreezeDate.Value - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
						ViewBag.CountdownLabel = "Code Freeze in:";
					}
					else if (endDate.HasValue)
					{
						unixTime = (int)(endDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
						ViewBag.CountdownLabel = "Rollout in:";
					}
					if (unixTime > 0)
						ViewBag.CountdownUnixTime = unixTime;
				}
			}

		}

		private void OnSaveStatus(object sender, ProjectStatusListEventArgs e)
		{
			var key = String.Format("{0}_{1}", e.CacheKey + ProjectStatusKeyPreffix, e.ProjectMilestone.Key);

			MsCache.Set(key, new ProjectStatusEx(e.Status));
		}

		private Dictionary<ProjectStatusListItem, ProjectStatusEx> GetLatestProjectStatuses(ref bool isComplete)
		{
			object storedKeysObject = null;
			string keys = string.Empty;

			if (MsCache.TryGet(FbAccountContext.Current.Url + ProjectStatusesKey, ref storedKeysObject))
				keys = (string)storedKeysObject;


			var result = new Dictionary<ProjectStatusListItem, ProjectStatusEx>();

			if (!String.IsNullOrEmpty(keys))
			{
				var splitKeys = keys.Split(',');

				foreach (var key in splitKeys)
				{
					var sessionKey = FbAccountContext.Current.Url + ProjectStatusKeyPreffix + "_" + key;
					object storedStatusObject = null;

					if (MsCache.TryGet(sessionKey, ref storedStatusObject))
					{

						var status = (ProjectStatusEx)storedStatusObject;

						var ids = key.Split('_');
						var projectMilesToneItem = new ProjectStatusListItem();
						projectMilesToneItem.ProjectId = Convert.ToInt32(ids[0]);
						projectMilesToneItem.MileStoneId = Convert.ToInt32(ids[1]);
						if (ids.Count() > 2)
							projectMilesToneItem.SubProjectParentCaseId = Convert.ToInt32(ids[2]);
						result.Add(projectMilesToneItem, status);
					}
				}

				isComplete = splitKeys.Count() == result.Count;
			}

			return result;
		}

		private void ClearProjectStatusesCacheData()
		{
			object storedKeysObject = null;
			string keys = string.Empty;

			if (MsCache.TryGet(FbAccountContext.Current.Url + ProjectStatusesKey, ref storedKeysObject))
				keys = (string)storedKeysObject;

			foreach (var key in keys.Split(','))
			{
				MsCache.TryRemove(FbAccountContext.Current.Url + ProjectStatusKeyPreffix + '_' + key);
			}

			MsCache.TryRemove(FbAccountContext.Current.Url + ProjectStatusesKey);
		}

		private HtmlString GenerateStatusLabel(ProjectStatusEx status, ProjectStatusListItem projectStatus)
		{
			var sb = new StringBuilder();

			sb.Append(String.Format("<span class=\"btn btn-{0}\" onclick=\"location.href='{1}/Project/Dashboard/{2}/{3}{4}'\">", status.Status.GetStatusClass(), EnvironmentConfig.GetFrontEndWebRootUrl(), projectStatus.ProjectId, projectStatus.MileStoneId, projectStatus.SubProjectParentCaseId.HasValue ? "/" + projectStatus.SubProjectParentCaseId.Value : ""));
			sb.Append(status.Status.GetStringValue());
			if (status.Status != FogbugzProjectStatus.NotStarted)
				sb.Append(String.Format(" ({0})", StringUtils.FormatSignDecimal(status.StatusValue, 1)));
			sb.Append("</span>");

			return new HtmlString(sb.ToString());
		}

	}

	internal class ProjectStatusEx
	{
		public FogbugzProjectStatus Status { get; set; }

		public decimal? StatusValue { get; set; }

		public ProjectStatusEx(ProjectStatus status)
		{
			Status = status.Status;
			StatusValue = status.StatusValue;

		}
	}
}

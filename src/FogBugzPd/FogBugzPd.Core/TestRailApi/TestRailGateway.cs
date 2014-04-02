using System;
using System.Collections.Generic;
using System.Linq;
using FogBugzPd.Core;
using TestRail.MiniAPI;
using TestRail.MiniAPI.Entities;

namespace FogBugzPd.Core.TestRailApi
{
	public static class TestRailGateway
	{
		private static string Token
		{
			get { return FbAccountContext.Current.Settings.TestRailConfig.Token; }
		}

		private static string Url
		{
			get { return FbAccountContext.Current.Settings.TestRailConfig.Url; }
		}

		private static readonly MiniAPIClient Client;

		static TestRailGateway()
		{
			Client = new MiniAPIClient(Url, Token);
		}

		#region | API Wrappers |

		public static List<Milestone> GetMilestones(int projectId)
		{
			List<Milestone> milestones;
			object rawMilestones = null;
			var key = "TR_Milestones_" + projectId.ToString();

			if (MsCache.TryGet(key, ref rawMilestones))
			{
				milestones = (List<Milestone>) rawMilestones;
			}
			else
			{
				milestones = Client.GetMilestones(projectId);
				MsCache.Set(key, milestones, TimeSpan.FromDays(1));

				foreach (var milestone in milestones)
				{
					MsCache.Set(GetMilestoneKey(milestone.Id), milestone, TimeSpan.FromDays(1));
				}
			}

			return milestones;
		}

		private static string GetMilestoneKey(int milestoneId)
		{
			return "TR_Milestone_" + milestoneId.ToString();
		}

		public static Milestone GetMilestone(int milestoneId)
		{
			Milestone milestone;
			object rawMilestone = null;
			var key = GetMilestoneKey(milestoneId);

			if (MsCache.TryGet(key, ref rawMilestone))
			{
				milestone = (Milestone) rawMilestone;
			}
			else
			{
				milestone = Client.GetMilestone(milestoneId);
				MsCache.Set(key, milestone, TimeSpan.FromDays(1));
			}

			return milestone;
		}

		public static List<Project> GetProjects()
		{
			List<Project> projects;
			object rawProjects = null;
			const string key = "TR_Projects";

			if (MsCache.TryGet(key, ref rawProjects))
			{
				projects = (List<Project>) rawProjects;
			}
			else
			{
				projects = Client.GetProjects();
				MsCache.Set(key, projects, TimeSpan.FromDays(1));
			}

			return projects;
		}

		public static Project GetProject(int projectId)
		{
			var projects = GetProjects();

			return projects.First(project => project.Id == projectId);
		}

		public static List<Plan> GetPlans(int projectId, int? milestoneId = null)
		{
			List<Plan> plans;
			object rawPlans = null;
			string key =
				milestoneId.HasValue
					? string.Format("TR_Plans_{0}_{1}", projectId, milestoneId)
					: string.Format("TR_Plans_{0}", projectId);

			plans = Client.GetPlans(projectId);
			if (MsCache.TryGet(key, ref rawPlans))
			{
				plans = (List<Plan>) rawPlans;
			}
			else
			{
				plans = Client.GetPlans(projectId);
				if (milestoneId.HasValue) plans = plans.Where(plan => plan.MilestoneId == milestoneId.Value).ToList();
				MsCache.Set(key, plans, TimeSpan.FromMinutes(5));
			}

			return plans;
		}

		public static List<Run> GetRuns(int projectId, int? planId = null)
		{
			List<Run> runs;
			object rawRuns = null;
			string key = planId.HasValue
				             ? string.Format("TR_Runs_{0}_{1}", projectId, planId)
				             : string.Format("TR_Runs_{0}", projectId);

			if (MsCache.TryGet(key, ref rawRuns))
			{
				runs = (List<Run>) rawRuns;
			}
			else
			{
				runs = Client.GetRuns(projectId, planId);
				MsCache.Set(key, runs, TimeSpan.FromMinutes(5));
			}

			return runs;
		}

		public static List<Test> GetTests(int runId)
		{
			List<Test> tests;
			object rawTests = null;
			string key = "TR_Tests_" + runId.ToString();

			if (MsCache.TryGet(key, ref rawTests))
			{
				tests = (List<Test>) rawTests;
			}
			else
			{
				tests = Client.GetTests(runId);
				MsCache.Set(key, tests, TimeSpan.FromMinutes(5));
			}

			return tests;
		}

		public static List<Case> GetCases(int suiteId)
		{
			List<Case> cases;
			object rawCases = null;
			string key = "TR_Cases_" + suiteId.ToString();

			if (MsCache.TryGet(key, ref rawCases))
			{
				cases = (List<Case>) rawCases;
			}
			else
			{
				cases = Client.GetCases(suiteId);
				foreach (var @case in cases)
				{
					var caseKey = @case.Id.ToString();
					MsCache.Set(caseKey, @case, TimeSpan.FromDays(28));
				}
				MsCache.Set(key, cases);
			}

			return cases;
		}

		public static Case GetCase(int caseId)
		{
			Case @case;

			object rawCase = null;
			string key = "TR_Case_" + caseId.ToString();

			if (MsCache.TryGet(key, ref rawCase))
			{
				@case = (Case) rawCase;
			}
			else
			{
				@case = Client.GetCase(caseId);
				MsCache.Set(key, @case, null, TimeSpan.FromDays(1));
			}

			return @case;
		}

		public static List<Suite> GetSuites(int projectId)
		{
			List<Suite> suites;
			object rawSuites = null;
			string key = "TR_Suites_" + projectId.ToString();

			if (MsCache.TryGet(key, ref rawSuites))
			{
				suites = (List<Suite>) rawSuites;
			}
			else
			{
				suites = Client.GetSuites(projectId);
				MsCache.Set(key, suites, TimeSpan.FromHours(1));
			}

			return suites;
		}

		#endregion
	}
}

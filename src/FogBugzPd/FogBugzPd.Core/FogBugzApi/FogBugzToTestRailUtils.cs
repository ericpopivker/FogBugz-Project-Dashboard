using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.TestRailApi;
using TestRail.MiniAPI.Entities;

namespace FogBugzPd.Core.FogBugzApi
{
	public static class FogBugzToTestRailUtils
	{
		private static readonly Regex EndingWithCFRegex = new Regex(@"^(?<name>.*?)\s*\(CF\s*\d{1,2}/\d{1,2}/\d{1,4}\)$", RegexOptions.Compiled);
		private static readonly Regex ProjectNameRegex = new Regex(@"^(?:(?:Project\s+)?[^-]+-\s+)?(?<name>.*)$", RegexOptions.Compiled);
		private static readonly Regex PlanEndsWithParantheses = new Regex(@"^(?<name>.*?)\s*(\(.*\))?$", RegexOptions.Compiled);

		public static TestRailPlansSummary GetTestRailPlansSummary(int milestoneId, int? projectCaseId)
		{
			/*
			return new TestRailPlansSummary
				{
					PassedCount = 10,
					BlockedCount = 10,
					RetestCount = 10,
					UntestedCount = 10,
					FailedCount = 10
				};
			*/

			var fbMilestone = FogBugzGateway.GetMilestone(milestoneId);

			// do not return anything for planning milestones
			if (fbMilestone.Name.ToLower().EndsWith("planning")) return null;

			var match = EndingWithCFRegex.Match(fbMilestone.Name);
			//if (!match.Success) return null;

			var fbMilestoneName = match.Success?match.Groups["name"].Value:fbMilestone.Name;

			var allTestRailMilestones = TestRailGateway.GetProjects().SelectMany(project => TestRailGateway.GetMilestones(project.Id));

			var trMilestone = allTestRailMilestones.FirstOrDefault(milestone => milestone.Name == fbMilestoneName);
			if (trMilestone == null) return null;

			var trProject = TestRailGateway.GetProject(trMilestone.ProjectId);

			var plans = TestRailGateway.GetPlans(trProject.Id, trMilestone.Id);

			if (!plans.Any()) return null;

			//if (projectCaseId.HasValue)
			//{
			//	var fbProjectCase = FogBugzUtils.GetCases(@"tag:""ProjectParentCase""").FirstOrDefault(@case => @case.Index == projectCaseId.Value);

			//	if (fbProjectCase == null) return null;

			//	var projectNameMatch = ProjectNameRegex.Match(fbProjectCase.Title);
			//	if (!projectNameMatch.Success) return null;

			//	var fbProjectCaseName = projectNameMatch.Groups["name"].Value;

			//	plans = plans.Where(plan =>
			//		{
			//			var m = PlanEndsWithParantheses.Match(plan.Name);
			//			var trPlanNameToMatch = m.Groups["name"].Value;
			//			return trPlanNameToMatch.Trim() == fbProjectCaseName.Trim();
			//		}).ToList();
			//	if (!plans.Any()) return null;
			//}

			var sw = new Stopwatch();

			sw.Start();
			// precache cases
			var suites = TestRailGateway.GetSuites(trProject.Id);
			sw.Stop();
			Debug.WriteLine("GetSuites took {0} ms", sw.ElapsedMilliseconds);
			
			sw.Reset();

			sw.Start();
			// get all those cases =, just to precache. They are intentionally not used right now
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			/*var casesBySuites = */
			suites.AsParallel().SelectMany(suite => TestRailGateway.GetCases(suite.Id)).ToList();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
			sw.Stop();
			Debug.WriteLine("Precaching took {0} ms", sw.ElapsedMilliseconds);
			sw.Reset();

			sw.Start();
			var runs = TestRailGateway.GetRuns(trProject.Id).Where(run => !run.IsCompleted && run.MilestoneId == trMilestone.Id).ToList();
			sw.Stop();
			Debug.WriteLine("Getting test runs took {0} ms", sw.ElapsedMilliseconds);
			sw.Reset();

			if (projectCaseId.HasValue)
			{
				var plan = plans.First();
				runs = runs.Where(run => run.PlanId == plan.Id).ToList();
			}

			sw.Start();
			var tests = runs.AsParallel().SelectMany(run => TestRailGateway.GetTests(run.Id)).ToList();
			sw.Stop();
			Debug.WriteLine("Getting tests took {0} ms for {1} test runs", sw.ElapsedMilliseconds, runs.Count);
			sw.Reset();

			sw.Start();
			var caseIdsWithTests = tests
				.GroupBy(test => test.CaseId)
				.Select(group => new
					{
						CaseId = group.Key,
						Test = group.OrderBy(test => test.Id).LastOrDefault()
					})
				.ToArray();
			sw.Stop();
			Debug.WriteLine("Grouping took {0} ms for {1} tests", sw.ElapsedMilliseconds, tests.Count);
			sw.Reset();

			sw.Start();
			var caseTests = caseIdsWithTests
				.Select(a => new
					{
						Case = TestRailGateway.GetCase(a.CaseId.Value),
						Test = a.Test
					})
				.ToArray();
			sw.Stop();
			Debug.WriteLine("Cases actualization took {0} ms for {1} cases", sw.ElapsedMilliseconds, caseTests.Count());

			var passedEstimate = caseTests.Where(caseTest => caseTest.Test.Status == TestStatus.Passed).Sum(caseTest => caseTest.Case.Estimate.GetValueOrDefault(0)) / 60;
			var blockedEstimate = caseTests.Where(caseTest => caseTest.Test.Status == TestStatus.Blocked).Sum(caseTest => caseTest.Case.Estimate.GetValueOrDefault(0)) / 60;
			var retestEstimate = caseTests.Where(caseTest => caseTest.Test.Status == TestStatus.Retest).Sum(caseTest => caseTest.Case.Estimate.GetValueOrDefault(0)) / 60;
			var untestedEstimate = caseTests.Where(caseTest => caseTest.Test.Status == TestStatus.Untested).Sum(caseTest => caseTest.Case.Estimate.GetValueOrDefault(0)) / 60;
			var failedEstimate = caseTests.Where(caseTest => caseTest.Test.Status == TestStatus.Failed).Sum(caseTest => caseTest.Case.Estimate.GetValueOrDefault(0)) / 60;

			return new TestRailPlansSummary
				{
					PassedCount = plans.Sum(plan => plan.PassedCount),
					BlockedCount = plans.Sum(plan => plan.BlockedCount),
					RetestCount = plans.Sum(plan => plan.RetestCount),
					UntestedCount = plans.Sum(plan => plan.UntestedCount),
					FailedCount = plans.Sum(plan => plan.FailedCount),
					PassedEstimate = passedEstimate,
					BlockedEstimate = blockedEstimate,
					RetestEstimate = retestEstimate,
					UntestedEstimate = untestedEstimate,
					FailedEstimate = failedEstimate
				};
		}
	}
}

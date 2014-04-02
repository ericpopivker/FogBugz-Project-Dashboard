using System;
using System.Collections.Generic;
using System.Linq;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Web.Models.Project
{
	public enum PriorityChartType
	{
		[StringValue("By Estimated Time")]
		ByTime = 1,
		[StringValue("By Cases Count")]
		ByCaseCount
	}

	public class PriorityViewModel : CaseSetViewModelBase
	{
		public List<PriorityListItem> Items { get; set; }

		public List<ChartItem> Charts { get; set; }

		public MsCacheInfo MsCache { get; set; }

		public PriorityViewModel()
		{
			Items = new List<PriorityListItem>();
			Charts = new List<ChartItem>();
			MsCache = new MsCacheInfo();
		}

		public void CreateItems()
		{
			var results = new List<PriorityListItem>();
			var casesGroups = CaseSet.Cases.GroupBy(c => c.PriorityName);

			foreach (var casesGroup in casesGroups)
			{
				var item = new PriorityListItem
					{
						Priority = casesGroup.Key,
						PriorityId = casesGroup.First().IndexPriority.Value,
						Total = casesGroup.Count()
					};

				var gCases = casesGroup.Where(c => c != null).ToList();

				var activeCases = gCases.Where(CaseUtils.IsActive).ToList();

				item.ActiveCount = activeCases.Count;

				item.WithoutEstimate = activeCases.Count(c => (c.IndexBugChildren==null || c.IndexBugChildren.Count()==0) && (!c.HoursCurrentEstimate.HasValue || c.HoursCurrentEstimate.Value == 0));

				item.EstimatedTime = activeCases.Sum(c => c.HoursCurrentEstimate ?? 0);

				item.ElapsedTime = activeCases.Sum(c => c.HoursElapsed ?? 0);

				item.RemainingTime = 0;
				foreach (var activeCase in activeCases)
				{
					var remaining = 0m;
					var estimate = activeCase.HoursCurrentEstimate ?? activeCase.HoursOriginalEstimate;
					if (estimate.HasValue)
					{
						remaining = estimate.Value - (activeCase.HoursElapsed ?? 0);
						if (remaining < 0) remaining = 0;
					}
					item.RemainingTime += remaining;
				}

				item.ClosedCount = gCases.Count(a => !a.IsOpen);

				item.ResolvedCount = gCases.Count(CaseUtils.IsResolved);

				item.VerifiedCount = gCases.Count(/*CaseUtils.IsResolvedVerified*/c => c.IndexStatus == FbAccountContext.Current.Settings.ResolvedVerifiedStatusId);

				results.Add(item);
			}
			Items =  results.OrderBy(i => i.PriorityId).ToList();
		}

		public void GenerateChartData()
		{
			Charts.Add(new ChartItem(PriorityChartType.ByTime, GetByTimeData(Items)));
			Charts.Add(new ChartItem(PriorityChartType.ByCaseCount, GetByCaseCountData(Items)));
		}

		private string GetByCaseCountData(List<PriorityListItem> items)
		{
			var ticks = new List<Array>();
			var dataActive = new List<Array>();
			var dataResolved = new List<Array>();
			var dataVerified = new List<Array>();
			var dataClosed = new List<Array>();

			var i = 0;
			foreach (var item in items)
			{
				dataActive.Add(new[] { i, item.ActiveCount });
				dataResolved.Add(new[] { i, item.ResolvedCount });
				dataVerified.Add(new[] { i, item.VerifiedCount });
				dataClosed.Add(new[] { i, item.ClosedCount });
				ticks.Add(new IConvertible[] { i + 0.5, String.Format("{0} - {1}", item.PriorityId, item.Priority) });
				i++;
			}

			var obj =
				new
				{
					Ticks = ticks,
					Series =
						new[]
								{
									new {label = "Active", data = dataActive}, new {label = "Resolved", data = dataResolved},
									new {label = "Verified", data = dataVerified}, new {label = "Closed", data = dataClosed}
								}
				};
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}

		private string GetByTimeData(List<PriorityListItem> items)
		{
			var ticks = new List<Array>();
			var dataEstimated = new List<Array>();
			var dataElapsed = new List<Array>();

			var i = 0;
			foreach (var item in items)
			{
				dataEstimated.Add(new[] { i, item.EstimatedTime });
				dataElapsed.Add(new[] { i, item.ElapsedTime });
				ticks.Add(new IConvertible[] { i + 0.5, String.Format("{0} - {1}", item.PriorityId, item.Priority) });
				i++;
			}

			var obj =
				new
				{
					Ticks = ticks,
					Series = new[] { new { label = "Estimated", data = dataEstimated }, new { label = "Elapsed", data = dataElapsed } }
				};
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}

		public override void Setup(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			base.Setup(projectId, milestoneId, subProjectParentCaseId);

			CreateItems();
			GenerateChartData();
		}
	}

	public class PriorityListItem
	{
		public string Priority { get; set; }

		public int PriorityId { get; set; }

		public int Total { get; set; }

		//Estimates
		public int WithoutEstimate { get; set; }

		public decimal EstimatedTime { get; set; }

		public decimal ElapsedTime { get; set; }

		public decimal RemainingTime { get; set; }

		//Statuses
		public int ActiveCount { get; set; }

		public int ResolvedCount { get; set; }

		public int ClosedCount { get; set; }

		public int VerifiedCount { get; set; }
	}

	public class ChartItem
	{
		public PriorityChartType Type { get; set; }

		public string Data { get; set; }

		public ChartItem(PriorityChartType type, string data)
		{
			Type = type;
			Data = data;
		}
	}
}

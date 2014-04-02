using System;
using System.Collections.Generic;
using System.Linq;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;


namespace FogBugzPd.Web.Models.Project
{
	public class DeveloperViewModel : CaseSetViewModelBase
	{
		public List<DeveloperListItem> Items { get; set; }

		public List<ChartItem> Charts { get; set; }

		public MsCacheInfo MsCache { get; set; }

		public DeveloperViewModel()
		{
			Items = new List<DeveloperListItem>();
			Charts = new List<ChartItem>();
			MsCache = new MsCacheInfo();
		}

		public void GenerateChartData()
		{
			Charts.Add(new ChartItem(PriorityChartType.ByTime, GetByTimeData(Items)));
			Charts.Add(new ChartItem(PriorityChartType.ByCaseCount, GetByCaseCountData(Items)));
		}

		private string GetByCaseCountData(List<DeveloperListItem> items)
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
				ticks.Add(new IConvertible[] { i + 0.5, item.Person });
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

		private string GetByTimeData(List<DeveloperListItem> items)
		{
			var ticks = new List<Array>();
			var dataEstimated = new List<Array>();
			var dataElapsed = new List<Array>();

			var i = 0;
			foreach (var item in items)
			{
				dataEstimated.Add(new[] { i, item.EstimatedTime });
				dataElapsed.Add(new[] { i, item.ElapsedTime });
				ticks.Add(new IConvertible[] { i + 0.5, item.Person });
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

		private void CreateItems()
		{
			var cases = CaseSet.Cases;

			var persons = FogBugzGateway.GetPersons();

			var items = new List<DeveloperListItem>();
			var casesGroups = cases.Where(c => c.IndexPersonAssignedTo != null).GroupBy(c => c.IndexPersonAssignedTo);

			foreach (var casesGroup in casesGroups)
			{
				var item = new DeveloperListItem {PersonId = casesGroup.Key.Value};

				var assignedTo = persons.SingleOrDefault(p => p.Index == item.PersonId);

				if(assignedTo!=null)
				{
					item.Person = assignedTo.Name;
				}
				
				item.Total = casesGroup.Count();

				var activeCases = casesGroup.Where(CaseUtils.IsActive).ToList();

				item.ActiveCount = activeCases.Count;

				item.WithoutEstimate = activeCases.Count(c => (!c.HoursCurrentEstimate.HasValue || c.HoursCurrentEstimate.Value == 0));

				item.EstimatedTime = activeCases.Sum(c => c.HoursCurrentEstimate ?? 0);

				item.ElapsedTime = activeCases.Sum(c => c.HoursElapsed ?? 0);

				item.RemainingTime = 0;
				foreach (var activeCase in activeCases)
				{
					var remaining = 0m;
					var estimate = activeCase.HoursCurrentEstimate ?? activeCase.HoursOriginalEstimate;
					if(estimate.HasValue)
					{
						remaining = estimate.Value - (activeCase.HoursElapsed??0);
						if (remaining < 0) remaining = 0;
					}
					item.RemainingTime += remaining;
				}

				item.ClosedCount = casesGroup.Count(CaseUtils.IsClosed);

				item.ResolvedCount = casesGroup.Count(CaseUtils.IsResolved);

				item.VerifiedCount = casesGroup.Count(/*CaseUtils.IsResolvedVerified*/c => c.IndexStatus == FbAccountContext.Current.Settings.ResolvedVerifiedStatusId);

				items.Add(item);
			}

			Items = items.OrderBy(i => i.Person).ToList();
		}

		public override void Setup(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			base.Setup(projectId, milestoneId, subProjectParentCaseId);

			CreateItems();
			GenerateChartData();
		}
	}

	public class DeveloperListItem
	{
		public string Person { get; set; }

		public int PersonId { get; set; }

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
}

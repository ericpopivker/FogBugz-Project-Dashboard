using System;
using System.Collections.Generic;
using System.ComponentModel;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Web.Models.Project
{
	public class AsyncDashboardViewModel : CaseSetViewModelBase
	{
		#region Intrernal classes
		public class DatesSectionViewModel
		{
			public bool IsActiveProject { get; set; }

			public class PlotTime
			{
				public string Label { get; set; }
				public DateTime DateTime { get; set; }

				public PlotTime(string label, DateTime dateTime)
				{
					Label = label;
					DateTime = dateTime;
				}
			}

			public DateTime? StartDate { get; set; }
			public int? StartDateDaysRemaining { get; set; }

			public DateTime? CodeFreeze { get; set; }
			public int? CodeFreezeDaysRemaining { get; set; }

			//public IList<Holiday> CodeFreezeHolidaysBefore { get; set; }
			public IList<TimeOffRange> CodeFreezeHolidaysBefore { get; set; }

			public DateTime? Rollout { get; set; }
			public int? RolloutDaysRemaining { get; set; }
			//public IList<Holiday> RolloutHolidaysBefore { get; set; }
			public IList<TimeOffRange> RolloutHolidaysBefore { get; set; }

			public DateTime MinimumTime { get; set; }
			public DateTime MaximumTime { get; set; }
			public DatesSectionViewModel.PlotTime[] PlotTimes { get; set; }
		}
		#endregion

		public DatesSectionViewModel DatesSection { get; private set; }
		public MsCacheInfo MsCache { get; set; }
		public ProjectStatus ProjectStatus { get; set; }

		public string CodeFreezeTip {
				get
				{
					
					if (MilestoneName.Contains("CF"))
						return null;
					if (!FbAccountContext.Current.Settings.AllowQaEstimates)
						return "Calculated by (Rollout - Start date) * (1 - (QA Time Percentage))";
					else
						return "Calculated by (Rollout - Start date) - (Total QA Time Estimates)";
				} 
			}

		public AsyncDashboardViewModel()
		{
			DatesSection = new DatesSectionViewModel();
			MsCache = new MsCacheInfo();
		}
	}
}

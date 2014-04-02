using System;
using System.Collections.Generic;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core;
using FogBugzPd.Core.ProjectStatus;

namespace FogBugzPd.Web.Models.Project
{
	public class AsyncDashboardNotStartedViewModel : CaseSetViewModelBase
	{
		#region inner classes
		public class CasesSectionViewModel
		{
			public int Total { get; set; }
			public int Active { get; set; }
			public int Resolved { get; set; }
			public int Verified { get; set; }
			public int Closed { get; set; }
		}

		public class TimeSectionViewModel
		{
			public decimal TotalEstimated { get; set; }
			public decimal Elapsed { get; set; }
			public decimal ActiveEstimated { get; set; }
			public decimal ActiveRemaining { get; set; }
		}

		public class EstimatesSectionViewModel
		{
			public int WithEstimates { get; set; }
			public int WithoutEstimates { get; set; }
			public int GoingOverEstimate { get; set; }
		}

		public class AccuracySectionViewModel
		{
			public decimal EstimatedTime { get; set; }
			public decimal ActualTime { get; set; }
		}

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
		public CasesSectionViewModel CasesSection { get; private set; }
		public TimeSectionViewModel TimeSection { get; private set; }
		public EstimatesSectionViewModel EstimatesSection { get; private set; }
		public AccuracySectionViewModel AccuracySection { get; private set; }
		public MsCacheInfo MsCache { get; set; }

		public AsyncDashboardNotStartedViewModel()
		{
			CasesSection = new CasesSectionViewModel();
			TimeSection = new TimeSectionViewModel();
			EstimatesSection = new EstimatesSectionViewModel();
			AccuracySection = new AccuracySectionViewModel();
			DatesSection = new DatesSectionViewModel();
			MsCache = new MsCacheInfo();
		}

		public DatesSectionViewModel DatesSection { get; private set; }
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
	}
}
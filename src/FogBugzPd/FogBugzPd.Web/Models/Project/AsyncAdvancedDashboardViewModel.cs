using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Web.Models.Project
{
	public class AsyncAdvancedDashboardViewModel: CaseSetViewModelBase
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
		#endregion
		public CasesSectionViewModel CasesSection { get; private set; }
		public TimeSectionViewModel TimeSection { get; private set; }
		public EstimatesSectionViewModel EstimatesSection { get; private set; }
		public AccuracySectionViewModel AccuracySection { get; private set; }
		public MsCacheInfo MsCache { get; set; }

		public AsyncAdvancedDashboardViewModel()
		{
			CasesSection = new CasesSectionViewModel();
			TimeSection = new TimeSectionViewModel();
			EstimatesSection = new EstimatesSectionViewModel();
			AccuracySection = new AccuracySectionViewModel();
			MsCache = new MsCacheInfo();
		}
	}
}
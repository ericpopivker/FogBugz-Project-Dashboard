using System;
using System.Collections.Generic;
using System.Linq;
using FogBugzPd.Core.TestRailApi;

namespace FogBugzPd.Web.Models.Project
{
	public class QAViewModel : CaseSetViewModelBase
	{
		// cases
		public int CasesTotal { get; set; }
		public int CasesWithoutTestEstimates { get; set; }
		public int CasesReadyToBeTested { get; set; }
		public int CasesActive { get; set; }
		public int CasesVerified { get; set; }
		public int CasesClosed { get; set; }
		public MsCacheInfo MsCache { get; set; }

		// times
		public decimal TotalTestingTime { get; set; }
		public decimal RemainingTestingTime { get; set; }
		public decimal ReadyToBeTestedTime { get; set; }

		public List<QAListItem> Items { get; private set; }

		public TestRailPlansSummary TestRailPlansSummary { get; set; }

		

		public QAViewModel()
		{
			Items = new List<QAListItem>();

			MsCache = new MsCacheInfo();
		}

		public void SetItems(IEnumerable<QAListItem> items)
		{
			Items.Clear();
			Items.AddRange(items.OrderByDescending(item => item.CaseToVerify).ThenBy(item => item.TesterName));
		}
	}

	public class QAListItem
	{
		public string TesterName { get; set; }
		public int CaseToVerify { get; set; }
		public decimal DevelopmentTime { get; set; }
		public decimal Estimate { get; set; }
		public int VerifiedCases { get; set; }
	}
}

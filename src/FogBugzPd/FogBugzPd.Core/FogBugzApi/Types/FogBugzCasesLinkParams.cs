using FogBugzPd.Core.FogBugzApi.Enums;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class FogBugzCasesLinkParams
	{
		public int? ProjectId { get; set; }
		public int? MilestoneId { get; set; }
		public int? SubProjectParentCaseId { get; set; }
		public int? PriorityId { get; set; }
		public int? StatusId { get; set; }
		public CaseStatus? Status { get; set; }
		public string PersonName { get; set; }
		public bool? HasEstimate { get; set; }
		public bool? HasChildren { get; set; }

		public string Tag { get; set; }
	}
}

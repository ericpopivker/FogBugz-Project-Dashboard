using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;


namespace FogBugzPd.Web.Models.Project
{
	public class CaseSetViewModelBase
	{
		private CaseSet _caseSet = null;

		public CaseSet CaseSet
		{
			get { return _caseSet; }
			set { _caseSet = value; }
		}

		public string ProjectName
		{
			get
			{
				if (_caseSet != null)
					return _caseSet.Project.Name;

				return FogBugzGateway.GetProject(this.ProjectId.Value).Name;
			}
		}

		public string MilestoneName
		{
			get { return Milestone.Name; }
		}

		public Milestone Milestone
		{
			get
			{
				if (_caseSet != null)
					return _caseSet.Milestone;

				return FogBugzGateway.GetMilestone(this.MilestoneId.Value);
			}
		}
		
		public string SubProjectParentCaseName
		{
			get
			{
				if (!SubProjectParentCaseId.HasValue)
					return null;

				if (_caseSet != null)
					return _caseSet.SubProjectParentCase.Title;
				
				return FogBugzGateway.GetSubProjectParentCase(this.SubProjectParentCaseId.Value).Title;
			}
		}

		//Disable for now
		public string WikiPageId
		{
			get
			{
				//if (Project.WikiPageId !=string.Empty) 
				//	return CaseSet.Project.WikiPageId;

				return null;
			}
		}


		public int? ProjectId { get; set; }

		public int? MilestoneId { get; set; }

		public int? SubProjectParentCaseId { get; set; }


		public virtual void Setup(int projectId, int milestoneId, int? subProjectParentCaseId)
		{
			Setup(projectId, milestoneId, subProjectParentCaseId, false);
		}

		public void Setup(int projectId, int milestoneId, int? subProjectParentCaseId, bool skipLoadingCaseSet)
		{
			ProjectId = projectId;
			MilestoneId = milestoneId;
			SubProjectParentCaseId = subProjectParentCaseId;

			if (!skipLoadingCaseSet)
				CaseSet = FogBugzGateway.GetCaseSet(ProjectId.Value, MilestoneId.Value, SubProjectParentCaseId);

		}
	}
}

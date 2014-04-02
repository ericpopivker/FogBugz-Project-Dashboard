using System.Collections.Generic;
using FogLampz.Model;

namespace FogBugzPd.Core.ProjectRetrieving
{
	public interface IProjectListModelProvider
	{
		ProjectMilestoneList Process(IList<Project> projects, IList<FixFor> milestones,
							  IList<Case> parentCases, int threadsCount);
	}
}
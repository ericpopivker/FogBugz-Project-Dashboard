using System.Collections.Generic;
using FogBugzPd.Core.FogBugzApi.Types;
using FogLampz.Model;
using Project = FogLampz.Model.Project;

namespace FogBugzPd.Core.FogBugzApi
{
	public class FogBugzClientCache
	{
		public IList<Project> Projects { get; set; }
		public IList<FixFor> Milestones { get; set; }
		public IList<Case> SubProjectParentCases { get; set; }
		public IList<Status> Statuses { get; set; }
		public IList<Person> Persons { get; set; }
		public FbSchedule Schedule { get; set; }
		public bool IsLoadedByAdminUser { get; set; }
		public List<Group> Groups { get; set; }
	}
}
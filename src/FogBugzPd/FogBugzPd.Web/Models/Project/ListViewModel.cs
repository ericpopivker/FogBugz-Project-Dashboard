using System.Collections.Generic;
using FogBugzPd.Core.FogBugzApi.Types;
using FogLampz.Model;

namespace FogBugzPd.Web.Models.Project
{
	public class ListViewModel
	{
		public List<ProjectListItem> Items { get; set; }

		public MsCacheInfo MsCache { get; set; }

		public List<Tag> AllActiveTags { get; set; }

		public ListViewModel()
		{
			Items = new List<ProjectListItem>();

			MsCache = new MsCacheInfo();
		}

		public bool ShowSubproject { get; set; }
	}


	public class ProjectListItem
	{

		public List<MilestoneListItem> MilestoneListItems { get; set; }

		public FogLampz.Model.Project Project { get; set; }

		public ProjectListItem()
		{
			MilestoneListItems = new List<MilestoneListItem>();
		}
	}

	public class MilestoneListItem
	{
		public FixFor Milestone { get; set; }

		public List<Case> SubProjectParentCases { get; set; }

		public MilestoneListItem()
		{
			SubProjectParentCases = new List<Case>();
		}
	}
}

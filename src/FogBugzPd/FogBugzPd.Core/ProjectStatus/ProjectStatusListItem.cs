using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FogLampz.Model;

namespace FogBugzPd.Core.ProjectStatus
{
	public class ProjectStatusListItem
	{
		public int ProjectId { get; set; }
		public Project Project { get; set; }

		public int MileStoneId { get; set; }
		public FixFor Milestone { get; set; }

		public int? SubProjectParentCaseId { get; set; }
		public Case SubProjectParentCase { get; set; }

		public string Key
		{
			get
			{
				return String.Format("{0}_{1}{2}", ProjectId, MileStoneId, SubProjectParentCaseId.HasValue ? "_" + SubProjectParentCaseId.Value : "");
			}
		}
	}
}

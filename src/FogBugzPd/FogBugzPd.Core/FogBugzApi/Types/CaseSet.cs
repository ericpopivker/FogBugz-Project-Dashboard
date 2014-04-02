using System.Collections.Generic;
using FogLampz.Model;


namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class CaseSet
	{
		public Project Project { get; set; }

		public Milestone Milestone { get; set; }

		public Case SubProjectParentCase { get; set; }
		
		public IList<Case> Cases { get; set; }
	}
}
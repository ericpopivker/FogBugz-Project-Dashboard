using FogBugzPd.Core.ProjectStatus;


namespace FogBugzPd.Application.Extensions
{
	public static class HtmlExtensions
	{
		public static string GetStatusClass(this FogbugzProjectStatus value)
		{
			switch (value)
			{
				case FogbugzProjectStatus.VeryBehind:
					return "verybehind";
				
				case FogbugzProjectStatus.LittleBehind:
					return "littlebehind";

				case FogbugzProjectStatus.OnTime:
					return "ontime";

				case FogbugzProjectStatus.ALittleAhead:
					return "littleahead";

				case FogbugzProjectStatus.VeryAhead:
					return "veryahead";

				case FogbugzProjectStatus.NotStarted:
					return "notstarted";
			}

			return "ontime";
		}
	}
}

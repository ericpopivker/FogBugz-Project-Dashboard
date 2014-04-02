using System;
using FogLampz.Model;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class Milestone
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public DateTime? DateStart { get; set; }
		public DateTime? DateRelease { get; set; }

		public Milestone(FixFor fixfor)
		{
			Id = fixfor.Index.Value;

			Name = fixfor.Name;

			DateStart = fixfor.DateStart;
			DateRelease = fixfor.Date;
		}
	}
}
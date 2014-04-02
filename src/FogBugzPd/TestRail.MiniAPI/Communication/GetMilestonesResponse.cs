using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetMilestonesResponse : ResponseBase
	{
		[JsonProperty("milestones")]
		public List<Milestone> Milestones { get; set; }
	}
}

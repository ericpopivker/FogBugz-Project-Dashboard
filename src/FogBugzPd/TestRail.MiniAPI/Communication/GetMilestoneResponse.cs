using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetMilestoneResponse : ResponseBase
	{
		[JsonProperty("milestone")]
		public Milestone Milestone { get; set; }
	}
}

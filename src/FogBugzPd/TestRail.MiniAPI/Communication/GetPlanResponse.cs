using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetPlanResponse : ResponseBase
	{
		[JsonProperty("plan")]
		public Plan Plan { get; set; }
	}
}

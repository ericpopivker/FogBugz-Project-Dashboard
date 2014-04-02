using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetProjectResponse : ResponseBase
	{
		[JsonProperty("project")]
		public Project Project { get; set; }
	}
}

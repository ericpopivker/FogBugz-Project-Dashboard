using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetTestResponse : ResponseBase
	{
		[JsonProperty("test")]
		public Test Test { get; set; }
	}
}

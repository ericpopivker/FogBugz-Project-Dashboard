using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetRunResponse : ResponseBase
	{
		[JsonProperty("run")]
		public Run Run { get; set; }
	}
}

using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetCaseResponse : ResponseBase
	{
		[JsonProperty("case")]
		public Case Case { get; set; }
	}
}

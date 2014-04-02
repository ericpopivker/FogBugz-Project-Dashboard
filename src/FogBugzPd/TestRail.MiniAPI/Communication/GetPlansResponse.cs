using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetPlansResponse : ResponseBase
	{
		[JsonProperty("plans")]
		public List<Plan> Plans { get; set; }
	}
}

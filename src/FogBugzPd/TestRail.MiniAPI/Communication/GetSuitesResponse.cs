using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetSuitesResponse : ResponseBase
	{
		[JsonProperty("suites")]
		public List<Suite> Suites { get; set; }
	}
}

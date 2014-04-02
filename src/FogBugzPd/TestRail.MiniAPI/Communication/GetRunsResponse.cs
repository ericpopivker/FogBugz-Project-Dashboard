using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetRunsResponse : ResponseBase
	{
		[JsonProperty("runs")]
		public List<Run> Runs { get; set; }
	}
}

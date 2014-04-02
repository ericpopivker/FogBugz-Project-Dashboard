using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetTestsResponse : ResponseBase
	{
		[JsonProperty("tests")]
		public List<Test> Tests { get; set; }
	}
}

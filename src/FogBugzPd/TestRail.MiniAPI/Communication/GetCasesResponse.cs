using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetCasesResponse : ResponseBase
	{
		[JsonProperty("cases")]
		public List<Case> Cases { get; set; }
	}
}

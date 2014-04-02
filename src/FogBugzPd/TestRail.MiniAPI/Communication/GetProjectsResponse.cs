using System.Collections.Generic;
using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetProjectsResponse : ResponseBase
	{
		[JsonProperty("projects")]
		public List<Project> Projects { get; set; }
	}
}

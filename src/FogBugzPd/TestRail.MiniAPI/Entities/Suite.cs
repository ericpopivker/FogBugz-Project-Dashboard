using Newtonsoft.Json;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Suite
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("project_id")]
		public int ProjectId { get; set; }
		
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
	}
}

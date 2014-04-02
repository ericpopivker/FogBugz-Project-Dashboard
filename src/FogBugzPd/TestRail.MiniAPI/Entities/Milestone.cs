using System;
using Newtonsoft.Json;
using TestRail.MiniAPI.Json.Converters;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Milestone
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("project_id")]
		public int ProjectId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("is_completed")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsCompleted { get; set; }

		[JsonProperty("due_on")]
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? DueOn { get; set; }

		[JsonProperty("completed_on")]
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? CompletedOn { get; set; }
	}
}

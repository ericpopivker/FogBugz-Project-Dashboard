using System;
using Newtonsoft.Json;
using TestRail.MiniAPI.Json.Converters;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Project
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("announcement")]
		public string Announcement { get; set; }
	
		[JsonProperty("is_completed")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsCompleted { get; set; }

		[JsonProperty("completed_on")]
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? CompletedOn { get; set; }
	}
}

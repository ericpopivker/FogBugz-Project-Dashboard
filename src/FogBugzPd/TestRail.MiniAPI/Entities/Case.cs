using Newtonsoft.Json;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Case
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("section_id")]
		public string SectionId { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("type_id")]
		public CaseType Type { get; set; }

		[JsonProperty("priority_id")]
		public int Priority { get; set; }

		[JsonProperty("estimate")]
		public int? Estimate { get; set; }

		[JsonProperty("estimate_forecast")]
		public int? EstimateForecast { get; set; }

		[JsonProperty("refs")]
		public string References { get; set; }
	}
}

using Newtonsoft.Json;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Test
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("status_id")]
		public TestStatus Status { get; set; }

		[JsonProperty("run_id")]
		public int RunId { get; set; }

		[JsonProperty("case_id")]
		public int? CaseId { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		public override string ToString()
		{
			return string.Format("{0} : {1}", Title, Status);
		}
	}
}

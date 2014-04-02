using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TestRail.MiniAPI.Json.Converters;

namespace TestRail.MiniAPI.Entities
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Run
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("project_id")]
		public int ProjectId { get; set; }

		[JsonProperty("milestone_id")]
		public int? MilestoneId { get; set; }

		[JsonProperty("plan_id")]
		public int? PlanId { get; set; }

		[JsonProperty("suite_id")]
		public int? SuiteId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("is_completed")]
		[JsonConverter(typeof(BoolConverter))]
		public bool IsCompleted { get; set; }

		[JsonProperty("completed_on")]
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? CompletedOn { get; set; }

		[JsonProperty("passed_count")]
		public int PassedCount { get; set; }

		[JsonProperty("blocked_count")]
		public int BlockedCount { get; set; }

		[JsonProperty("untested_count")]
		public int UntestedCount { get; set; }

		[JsonProperty("retest_count")]
		public int RetestCount { get; set; }

		[JsonProperty("failed_count")]
		public int FailedCount { get; set; }
	}
}

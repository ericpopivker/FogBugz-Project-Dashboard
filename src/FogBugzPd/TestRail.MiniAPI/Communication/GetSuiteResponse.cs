using Newtonsoft.Json;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class GetSuiteResponse : ResponseBase
	{
		[JsonProperty("suite")]
		public Suite Suite { get; set; }
	}
}

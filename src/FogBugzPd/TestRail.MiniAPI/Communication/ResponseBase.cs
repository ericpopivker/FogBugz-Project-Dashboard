using Newtonsoft.Json;

namespace TestRail.MiniAPI.Communication
{
	[JsonObject(MemberSerialization.OptIn)]
	public class ResponseBase
	{
		[JsonProperty("result")]
		public bool Result { get; set;}

		[JsonProperty("error")]
		public string Error { get; set; }
	}
}

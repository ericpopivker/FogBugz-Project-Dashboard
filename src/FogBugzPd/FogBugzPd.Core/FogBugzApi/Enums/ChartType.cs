using FogBugzPd.Infrastructure;

namespace FogBugzPd.Core.FogBugzApi.Enums
{
	public enum ChartType
	{
		[StringValue("By Time")]
		ByTime = 1,
		[StringValue("By Cases Count")]
		ByCaseCount
	}
}
using FogBugzPd.Core.FogBugzApi.Enums;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class ChartItem
	{
		public ChartType Type { get; set; }

		public string Data { get; set; }

		public ChartItem(ChartType type, string data)
		{
			Type = type;
			Data = data;
		}
	}
}
using System.Linq;
using System.Web.Mvc;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;
using StackExchange.Profiling.SqlFormatters;

namespace FogBugzPd.Web.Utils
{

	public class MiniProfilerUtils
	{
		public static void CreateProfilingActionFilter()
		{
			// Add Profiling Action Filter (mvc mini profiler)
			GlobalFilters.Filters.Add(new ProfilingActionFilter());

			// Add Profiling View Engine (mvc mini profiler)
			var copy = ViewEngines.Engines.ToList();
			ViewEngines.Engines.Clear();
			foreach (var item in copy)
			{
				ViewEngines.Engines.Add(new ProfilingViewEngine(item));
			}
		}

		public static void Init()
		{
			MiniProfiler.Settings.PopupMaxTracesToShow = 10;
			MiniProfiler.Settings.ShowControls = true;
			MiniProfiler.Settings.SqlFormatter = new SqlServerFormatter();
		}
	}
}
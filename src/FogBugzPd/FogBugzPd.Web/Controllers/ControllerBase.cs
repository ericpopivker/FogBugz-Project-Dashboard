using System;
using FogBugzPd.Core;
using System.Web.Mvc;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Web.Utils;
using StackExchange.Profiling;

namespace FogBugzPd.Web.Controllers
{
	public class ControllerBase : Controller
	{
		protected FogBugzPdDbContext DbContext
		{
			get { return FogBugzPdDbContext.Current; }
		}


		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//Session is available at this point, so check if MiniProfiler should be visible
			if (!EnvironmentConfig.UseMiniProfiler)
				MiniProfiler.Stop(discardResults: true);

			base.OnActionExecuting(filterContext);
		}
	}
}
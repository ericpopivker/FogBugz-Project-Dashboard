using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FogBugzPd.Application;
using FogBugzPd.Core;
using FogBugzPd.Core.Migrations;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Attributes;
using FogBugzPd.Web.Controllers;
using FogBugzPd.Web.Utils;
using FogLampz;
using StackExchange.Profiling;

namespace FogBugzPd.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			if (EnvironmentConfig.UseMiniProfiler)
			{
				MiniProfilerUtils.CreateProfilingActionFilter();
				MiniProfilerUtils.Init();
				MiniProfilerEF.Initialize();
			}

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			FbAccountContext.GetContextFbAccountIdMethod = GetContextFbAccountId;

			FogBugzPdDbContext.RunMigrations();
		}


		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new DbContextFilterAttribute());
			filters.Add(new HandleErrorAttribute());

			if (EnvironmentConfig.RequireHttps)
				filters.Add(new RequireHttpsAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Environment",
				"Environment",
				new {controller = "Home", action = "Environment"}
				);

			routes.MapRoute(
				"ProjectAndMilestoneAndSubProject", // Route name
				"Project/{action}/{projectId}/{milestoneId}/{subProjectParentCaseId}", // URL with parameters
				new { controller = "Project", action = "Index" }, // Parameter defaults
				new { milestoneId = @"\d+", projectId = @"\d+", subProjectParentCaseId = @"\d+" }
				);
			
			routes.MapRoute(
				"ProjectAndMilestone", // Route name
				"Project/{action}/{projectId}/{milestoneId}", // URL with parameters
				new { controller = "Project", action = "Index" }, // Parameter defaults
				new { milestoneId = @"\d+", projectId = @"\d+" }
				);

			routes.MapRoute(
				"DashboardNotStarted", // Route name
				"Project/DashboardNotStarted/{projectId}/{milestoneId}", // URL with parameters
				new { controller = "Project", action = "Index" }, // Parameter defaults
				new { milestoneId = @"\d+", projectId = @"\d+" }
				);

			routes.MapRoute(
				"DailyDigestEmail", // Route name
				"Agent/SendDailyDigestEmails/{guid}", // URL with parameters
				new { controller = "Agent", action = "SendDailyDigestEmails" } // Parameter defaults
				);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
				);
		}

		private int GetContextFbAccountId()
		{
			if (UserContext.FbAccountId == null)
				throw new ArgumentNullException("UserContext.FbAccountId");

			return UserContext.FbAccountId.Value;
		}

		protected void Application_PreRequestHandlerExecute()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}


		protected void Application_Error(object sender, EventArgs e)
		{
			var httpContext = HttpContext.Current;
			var ex = httpContext.Server.GetLastError();

			ErrorUtils.HandleException(ex);

			if (!EnvironmentConfig.UseErrorLog)
				return;

			var code = 500;
			if (ex is HttpException)
			{
				code = ((HttpException)ex).GetHttpCode();
			}

			if (code == 404 || code == 500)
			{
				Response.Clear();
				
				var routeData = new RouteData();
				routeData.Values.Add("controller", "Home");
				routeData.Values.Add("action", "Error");
				routeData.Values.Add("code", code.ToString());
				Server.ClearError();

				// Avoid IIS7 getting in the middle
				Response.TrySkipIisCustomErrors = true;

				// Call target Controller and pass the routeData.
				IController errorController = new HomeController();
				errorController.Execute(new RequestContext(
											new HttpContextWrapper(Context), routeData));
			}
		}

		protected void Application_EndRequest()
		{
			if (EnvironmentConfig.UseMiniProfiler)  MiniProfiler.Stop();
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			if (EnvironmentConfig.UseMiniProfiler) MiniProfiler.Start();
		}

	}
}

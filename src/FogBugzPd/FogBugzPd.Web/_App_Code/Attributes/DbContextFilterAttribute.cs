using System.Web;
using System.Web.Mvc;
using FogBugzPd.Core;

namespace FogBugzPd.Web.Attributes
{
	/// <summary>
	/// This approach is based on ideas here:
	/// http://ayende.com/Blog/archive/2011/04/12/refactoring-toward-frictionless-amp-odorless-code-what-about-transactions.aspx
	/// </summary>
	public class DbContextFilterAttribute : ActionFilterAttribute
	{
		//6/26/2011 Eric - Need to use HttpContext.Current.Items, since adding internal _dbContextScope was not thread safe
		private static FogBugzPdDbContextScope DbContextScope
		{
			get { return HttpContext.Current.Items["DbContextFilterAttribute.DbContextScope"] as FogBugzPdDbContextScope; }
			set { HttpContext.Current.Items["DbContextFilterAttribute.DbContextScope"] = value; }
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			DbContextScope = new FogBugzPdDbContextScope();
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			DbContextScope.Dispose();
			DbContextScope = null;
		}

	}
}
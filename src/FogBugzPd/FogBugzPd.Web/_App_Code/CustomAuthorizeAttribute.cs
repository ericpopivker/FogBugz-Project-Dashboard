using System;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Web
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		private bool _isAuthorized;

		//http://stackoverflow.com/questions/1916087/asp-net-mvc-2-error-redirecting-from-actionfilter
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectResult(UrlUtils.LoginUrl());
		}


		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!UserContext.IsLoggedIn)
			{
				_isAuthorized = false;
				return _isAuthorized;
			}

			_isAuthorized = true;
			return _isAuthorized;
		}
	}
}
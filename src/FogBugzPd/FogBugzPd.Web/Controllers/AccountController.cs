using System;
using System.Web.Mvc;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Web.Models.Account;
using FogBugzPd.Web.Utils;
using FogLampz;

namespace FogBugzPd.Web.Controllers
{
	public class AccountController : ControllerBase
	{
		public enum LoginResult
		{
			Success = 1,
			LocalLoginFailed,
			ForBugzLoginFailed,
			Skipped
		}


		[HttpGet]
		public ActionResult Login()
		{
			var tenantConfig = EnvironmentConfig.TenantConfiguration;

			var result = TryLogin(tenantConfig);

			if (result == LoginResult.Success)
			{
				return RedirectToAction("Index", "Project");
			}
			else if (result == LoginResult.LocalLoginFailed)
			{
				return RedirectToAction("LocalLogin");
			}
			else if (result == LoginResult.ForBugzLoginFailed)
			{
				throw new ApplicationException("Invalid FogBugz credentials passed.");
			}

			var model = new LoginViewModel();
			return View(model);
		}


		public LoginResult TryLogin(TenantConfiguration configuration)
		{
			if (configuration.Mode == TenantMode.Single)
			{
				var isLocalUserLoginPassed = !configuration.LocalLogin_Enabled || (configuration.LocalLogin_Username == EnvironmentConfig.LocalLoginUsername
																				   && configuration.LocalLogin_Password == EnvironmentConfig.LocalLoginPassword);

				if (isLocalUserLoginPassed)
				{
					return LoginUtils.LoginToFogBugz(configuration.FogBugz_Url, configuration.FogBugz_Username, configuration.FogBugz_Password)
							   ? LoginResult.Success
							   : LoginResult.ForBugzLoginFailed;
				}

				return LoginResult.LocalLoginFailed;
			}

			return LoginResult.Skipped;

		}


		[HttpGet]
		public ActionResult LocalLogin()
		{
			return View(new LocalLoginViewModel());
		}

		[HttpPost]
		public ActionResult LocalLogin(LocalLoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var tenantConfig = EnvironmentConfig.TenantConfiguration;

			tenantConfig.LocalLogin_Username = model.UserName;
			tenantConfig.LocalLogin_Password = model.Password;

			var result = TryLogin(tenantConfig);

			if (result == LoginResult.Success)
			{
				return RedirectToAction("Index", "Project");
			}
			else if (result == LoginResult.LocalLoginFailed)
			{
				model.ErrorMessage = "Login failed for given username/password";
				return View(model);
			}
			else if (result == LoginResult.ForBugzLoginFailed)
			{
				throw new ApplicationException("FogBugz credentials supplied are invalid.");
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			string fogBugzUrl = model.FogBugzUrl.ToLower();

			//remove backslash from end if there is one
			fogBugzUrl = fogBugzUrl.TrimEnd(new[] { '/' });

			if (!LoginUtils.LoginToFogBugz(fogBugzUrl, model.FogBugzUsername, model.FogBugzPassword))
			{
				model.ErrorMessage = "Login to FogBugz failed for given url, username and password.";
				return View(model);
			}

			FogBugzClient.LogOn(fogBugzUrl + "/api.asp", model.FogBugzUsername, model.FogBugzPassword);

			return RedirectToAction("Index", "Home");
		}


		[HttpPost]
		public ActionResult DemoLogin()
		{
			if (!LoginUtils.LoginToFogBugz(Const.FogBugzDemoUrl, Const.FogBugzDemoUsername, Const.FogBugzDemoPassword))
				throw new InvalidOperationException("Demo account is not available.");

			FogBugzClient.LogOn(Const.FogBugzDemoUrl + "/api.asp", Const.FogBugzDemoUsername, Const.FogBugzDemoPassword);

			return RedirectToAction("Index", "Project");
		}

		

		[CustomAuthorize]
		public ActionResult Logout()
		{
			UserContext.ClearLoggedInUser();

			return RedirectToAction("Index", "Home");
		}

	}
}

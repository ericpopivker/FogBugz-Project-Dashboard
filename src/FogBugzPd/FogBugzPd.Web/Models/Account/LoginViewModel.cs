using System.ComponentModel.DataAnnotations;
using UrlAttribute = FogBugzPd.Web.CustomValidation.UrlAttribute;


namespace FogBugzPd.Web.Models.Account
{
	public class LoginViewModel
	{
		/*[Display(Name = "User Name")]
		[Required]
		public string UserName { get; set; }

		[Display(Name = "Password")]
		[Required]
		public string Password { get; set; }*/

		public string ErrorMessage { get; set; }

		[Display(Name = "FogBugz Url")]
		[Required]
		[CustomValidation.UrlAttribute]
		public string FogBugzUrl { get; set; }

		[Display(Name = "FogBugz Username")]
		[Required]
		public string FogBugzUsername { get; set; }

		[Display(Name = "FogBugz Password")]
		[Required]
		public string FogBugzPassword { get; set; }

		/*[Display(Name = "Remember me")]
		public bool RememberMe { get; set; }*/

		public string DemoErrorMessage { get; set; }

		[Display(Name = "FogBugz Demo Url")]
		[Required]
		[CustomValidation.UrlAttribute]
		public string FogBugzDemoUrl = Const.FogBugzDemoUrl; 

		[Display(Name = "FogBugz Demo Username")]
		[Required]
		public string FogBugzDemoUsername = Const.FogBugzDemoUsername; 

		[Display(Name = "FogBugz Demo Password")]
		[Required]
		public string FogBugzDemoPassword = Const.FogBugzDemoPassword; 

	}
}

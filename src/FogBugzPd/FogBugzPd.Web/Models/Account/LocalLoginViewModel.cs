using System.ComponentModel.DataAnnotations;

namespace FogBugzPd.Web.Models.Account
{
	public class LocalLoginViewModel
	{
		[Display(Name = "User Name")]
		[Required]
		public string UserName { get; set; }

		[Display(Name = "Password")]
		[Required]
		public string Password { get; set; }

		public string ErrorMessage { get; set; }

		[Display(Name = "Remember me")]
		public bool RememberMe { get; set; }
	}
}
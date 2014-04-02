using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FogLampz;

namespace FogBugzPd.Core.FbAccountModule
{
	[Table("FbAccount")]
	public class FbAccount
	{
		#region Properties

		public int Id { get; set; }

		//(unique, has common format all lower case, no / at the end)
		[Required]
		public string Url { get; set; }

		public string Token { get; set; }

		public virtual FbAccountSettings Settings { get; set; }

		#endregion

		public FbAccount()
		{
			Settings = new FbAccountSettings();
		}
	}
}

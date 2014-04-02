using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FogBugzPd.Core.FbAccountModule
{
	[Table("TestRailConfiguration")]
	public class TestRailConfiguration
	{
		public int Id { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		[MaxLength(32)]
		public string Token { get; set; }
	}
}

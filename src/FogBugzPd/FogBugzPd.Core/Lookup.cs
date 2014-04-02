using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FogBugzPd.Core
{
	public abstract class Lookup
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[Required()]
		[MaxLength(100)]
		public string Name { get; set; }
	}
}
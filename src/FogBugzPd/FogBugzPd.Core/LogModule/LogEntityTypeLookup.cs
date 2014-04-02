using System.ComponentModel.DataAnnotations.Schema;
using FogBugzPd.Core;

namespace FogBugzPd.Core.LogModule
{

	public enum LogEntityType
	{
		Agent = 1
	}

	[Table("LogEntityType")]
	public class LogEntityTypeLookup : Lookup
	{
	}
}

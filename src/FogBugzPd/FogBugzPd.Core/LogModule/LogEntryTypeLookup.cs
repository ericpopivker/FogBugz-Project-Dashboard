using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FogBugzPd.Core.LogModule
{
	public enum LogEntryType
	{
		//Agent
		Agent_DailyDigest = 1,
		//User
		User_Login = 201,
	}
	
	[Table("LogEntryType")]
	public class LogEntryTypeLookup : Lookup
	{
		[Required]
		public int EntityTypeId { get; set; }

		public LogEntityType EntityType
		{
			get { return (LogEntityType) EntityTypeId; }
			set { EntityTypeId = (int) value; }
		}

		[ForeignKey("EntityTypeId")]
		public virtual LogEntityTypeLookup EntityTypeLookup { get; set; }

	}
}

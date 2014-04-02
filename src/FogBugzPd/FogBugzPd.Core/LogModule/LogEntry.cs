using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FogBugzPd.Core.LogModule
{
	//For each entity type start new IDs group 100 each

	[Table("LogEntry")]
	public class LogEntry
	{
		public int Id { get; set; }

		//[NotMapped]
		//[Required]
		//public LogEntityType EntityType			
		//{
		//    get { return (LogEntityType)EntityTypeId; }
		//    set { EntityTypeId = (int)value; }
		//}

		//[Required]
		////public int EntityTypeId { get; set; }

		//[ForeignKey("EntityTypeId")]
		//public virtual LogEntityTypeLookup EntityTypeLookup { get; set; }

		[NotMapped]
		[Required]
		public LogEntryType Type
		{
			get { return (LogEntryType)TypeId; }
			set { TypeId = (int)value; }
		}

		[Required]
		public int TypeId { get; set; }

		[ForeignKey("TypeId")]
		public virtual LogEntryTypeLookup TypeLookup { get; set; }  

		[MaxLength]
		public string Description { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }
	}
}

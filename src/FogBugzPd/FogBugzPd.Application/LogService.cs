using System;
using FogBugzPd.Core.LogModule;

namespace FogBugzPd.Application
{
	public class LogService : ServiceBase
	{
		public void AddEntry(LogEntryType type, string description)
		{
			var logEntry = new LogEntry
							{
								//EntityType = entityType,
								Type = type,
								Description = description,
								CreatedAt = DateTime.Now
							};

			DbContext.LogEntries.Add(logEntry);
		   
			DbContext.SaveChanges();
		}
	}
}

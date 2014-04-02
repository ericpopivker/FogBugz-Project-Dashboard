using System.Data.Entity.Migrations;
using FogBugzPd.Core.LogModule;

namespace FogBugzPd.Core.Migrations
{
	public sealed class Configuration : DbMigrationsConfiguration<FogBugzPd.Core.FogBugzPdDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(FogBugzPd.Core.FogBugzPdDbContext context)
		{
			context
				.LogEntityTypes
				.AddOrUpdate(t => t.Id,
							 new LogEntityTypeLookup() { Id = 1, Name = "Agent" }
				);

			context
				.LogEntryTypes
				.AddOrUpdate(l => l.Id,
							 new LogEntryTypeLookup() { Id = 1, Name = "Send Daily Digest Email", EntityTypeId = 1 }
				);
		}
	}
}

namespace FogBugzPd.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyDigestEmailGuidRename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FbAccountSettings", "Guid", c => c.Guid());
            DropColumn("dbo.FbAccountSettings", "DailyDigestEmailGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FbAccountSettings", "DailyDigestEmailGuid", c => c.Guid());
            DropColumn("dbo.FbAccountSettings", "Guid");
        }
    }
}

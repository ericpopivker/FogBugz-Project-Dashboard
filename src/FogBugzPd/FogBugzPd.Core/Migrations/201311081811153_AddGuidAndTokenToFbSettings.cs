namespace FogBugzPd.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidAndTokenToFbSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FbAccount", "Token", c => c.String());
            AddColumn("dbo.FbAccountSettings", "DailyDigestEmailGuid", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FbAccountSettings", "DailyDigestEmailGuid");
            DropColumn("dbo.FbAccount", "Token");
        }
    }
}

namespace FogBugzPd.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDailyEmailsToFbAccountSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FbAccountSettings", "SendDailyDigestEmails", c => c.Boolean(nullable: false));
            AddColumn("dbo.FbAccountSettings", "SendDailyDigestEmailsTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FbAccountSettings", "SendDailyDigestEmailsTo");
            DropColumn("dbo.FbAccountSettings", "SendDailyDigestEmails");
        }
    }
}

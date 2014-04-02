namespace FogBugzPd.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FB2573_AllowQaEstimates_QaEstimateCustomFieldname2FbAccountSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FbAccountSettings", "AllowQaEstimates", c => c.Boolean(nullable: false));
            AddColumn("dbo.FbAccountSettings", "QaEstimateCustomFieldname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FbAccountSettings", "QaEstimateCustomFieldname");
            DropColumn("dbo.FbAccountSettings", "AllowQaEstimates");
        }
    }
}

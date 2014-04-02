namespace FogBugzPd.Core.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class FB2423_ChangeResovledVerifiedStatusType : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.FbAccountSettings", "ResolvedVerifiedStatusId", c => c.Int(nullable: false, defaultValueSql: "0"));
			DropColumn("dbo.FbAccountSettings", "ResolvedVerifiedStatus");
		}

		public override void Down()
		{
			AddColumn("dbo.FbAccountSettings", "ResolvedVerifiedStatus", c => c.String());
			DropColumn("dbo.FbAccountSettings", "ResolvedVerifiedStatusId");
		}
	}
}

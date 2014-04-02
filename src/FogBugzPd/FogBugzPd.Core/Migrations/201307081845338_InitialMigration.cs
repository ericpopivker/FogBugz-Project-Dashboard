namespace FogBugzPd.Core.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class InitialMigration : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"TestRailConfiguration",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					Url = c.String(nullable: false),
					Token = c.String(nullable: false, maxLength: 32),
				})
				.PrimaryKey(t => t.Id);

			CreateTable(
				"FbAccountSettings",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					ResolvedVerifiedStatus = c.String(),
					AllowSubProjects = c.Boolean(nullable: false),
					QaPercentage = c.Int(nullable: false),
					AllowTestRail = c.Boolean(nullable: false),
					SubProjectTag = c.String(),
					TestRailConfigId = c.Int(),
				})
				.PrimaryKey(t => t.Id);

			AddForeignKey("FbAccountSettings", "TestRailConfigId", "TestRailConfiguration", "Id");

			CreateIndex("FbAccountSettings", "TestRailConfigId");

			CreateTable(
				"FbAccount",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						Url = c.String(nullable: false),
						SettingsId = c.Int(nullable: false),
					})
				.PrimaryKey(t => t.Id);

			AddForeignKey("FbAccount", "SettingsId", "FbAccountSettings", "Id");

			CreateIndex("FbAccount", "SettingsId");
		}

		public override void Down()
		{
			DropIndex("dbo.FbAccountSettings", new[] { "TestRailConfigId" });
			DropIndex("dbo.FbAccount", new[] { "SettingsId" });
			DropForeignKey("dbo.FbAccountSettings", "TestRailConfigId", "dbo.TestRailConfiguration");
			DropForeignKey("dbo.FbAccount", "SettingsId", "dbo.FbAccountSettings");
			DropTable("dbo.TestRailConfiguration");
			DropTable("dbo.FbAccountSettings");
			DropTable("dbo.FbAccount");
		}
	}
}

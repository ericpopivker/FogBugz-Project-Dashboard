namespace FogBugzPd.Core.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class FB2564_Rename_TestRailConfigId : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "FbAccountSettings", name: "TestRailConfigId", newName: "TestRailConfigurationId");
		}

		public override void Down()
		{
			RenameColumn(table: "FbAccountSettings", name: "TestRailConfigurationId", newName: "TestRailConfigId");
		}
	}
}

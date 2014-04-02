namespace FogBugzPd.Core.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class FB2654_ChangeQaEstimateCustomFieldname_To_QaEstimateCustomFieldName : DbMigration
	{
		public override void Up()
		{
			RenameColumn("dbo.FbAccountSettings", "QaEstimateCustomFieldname", "QaEstimateCustomFieldName");
		}

		public override void Down()
		{
			RenameColumn("dbo.FbAccountSettings", "QaEstimateCustomFieldName", "QaEstimateCustomFieldname");
		}
	}
}

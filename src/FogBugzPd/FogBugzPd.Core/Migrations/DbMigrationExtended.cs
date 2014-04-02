using System;

namespace FogBugzPd.Core.Migrations
{
	/// <summary>
	/// Override naming conventions
	/// </summary>
	public abstract class DbMigration : System.Data.Entity.Migrations.DbMigration
	{
		public new void CreateIndex(string table, string column, bool unique = false, string name = null,
									object anonymousArguments = null)
		{
			if (String.IsNullOrEmpty(name))
				name = GetIndexName(table, column, unique);

			base.CreateIndex(table, column, unique, name, anonymousArguments);
		}

		public new void AddForeignKey(string dependentTable, string dependentColumn, string principalTable,
									  string principalColumn = null, bool cascadeDelete = false, string name = null,
									  object anonymousArguments = null)
		{
			if (String.IsNullOrEmpty(name))
				name = GetForeignKeyName(dependentTable, principalTable);

			base.AddForeignKey(dependentTable, dependentColumn, principalTable, principalColumn, cascadeDelete, name,
							   anonymousArguments);
		}

		public void DropForeignKey(string dependentTable, string dependentColumn, string principalTable)
		{
			try
			{
				var name = GetForeignKeyName(dependentTable, principalTable);
				DropForeignKey(dependentTable, name);
			}
			catch (Exception)
			{
				base.DropForeignKey(dependentTable, dependentColumn, principalTable);
			}
		}

		public void DropIndex(string table, string[] columns)
		{

			try
			{
				var name = GetIndexName(table, columns[0]);
				DropIndex(table, name);
			}
			catch (Exception)
			{
				base.DropIndex(table, columns);
			}
		}

		public static string GetForeignKeyName(string dependentTable, string principalTable)
		{
			return String.Format("FK_{0}_{1}", dependentTable, principalTable);
		}

		public static string GetIndexName(string table, string column, bool? unique = null)
		{
			unique = unique.HasValue && unique.Value;
			return String.Format("{0}_{1}_{2}", unique.Value ? "UQ" : "IX", table, column);
		}

	}
}

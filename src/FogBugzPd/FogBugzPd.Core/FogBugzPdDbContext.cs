using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Core.LogModule;
using FogBugzPd.Web.Utils;
using FogBugzPd.Web._App_Code.Utils;

namespace FogBugzPd.Core
{
	public class FogBugzPdDbContext : DbContext
	{

		public static FogBugzPdDbContext Current
		{
			get
			{
				return (FogBugzPdDbContext)XContextDataUtils.GetData("FogBugzPdDbContext.Current");
			}

			set
			{
				XContextDataUtils.SetData("FogBugzPdDbContext.Current", value);
			}
		}

		public FogBugzPdDbContext()
			: base(EnvironmentConfig.ConnectionStringName)
		{
		}


		//---------------------------------------//

		public static void DropCreateDatabaseIfModelChanges()
		{
			System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FogBugzPdDbContext>());

			EnsureDbCreated();
		}

		public static void DropCreateDatabaseAlways()
		{
			System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<FogBugzPdDbContext>());

			EnsureDbCreated();
		}

		private static void EnsureDbCreated()
		{
			//Ensure that DB is created at his point
			//Need to do this or TransactionScope in TestInitialize will cause MSDTC error when DbContext is actually used
			//for the first time, since that's when the DB creation is deferred to.
			using (var db = new FogBugzPdDbContext())
			{
				//This will create database
				//db.Users.FirstOrDefault(usr => usr.Id == 1); //Commented for now because of no USER Module
			}
		}

		private static void DeleteAll<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
		{
			foreach (TEntity entity in dbSet.ToList())
			{
				dbSet.Remove(entity);
			}
		}

		public bool ValidateEntity<TEntity>(TEntity entity) where TEntity : class
		{
			DbEntityValidationResult res = Current.Entry<TEntity>(entity).GetValidationResult();
			if (!res.IsValid)
			{
				var e = new DbEntityValidationException
					("Entity validation failed", new List<DbEntityValidationResult> { res });
				throw e;
			}
			return true;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<FbAccount>()
			            .HasRequired(c => c.Settings)
						.WithMany()
						.Map(c => c.MapKey("SettingsId"))
						.WillCascadeOnDelete(true);


			modelBuilder.Entity<FbAccountSettings>()
				.HasOptional(d => d.TestRailConfig)
				.WithMany()
				.Map(d => d.MapKey("TestRailConfigurationId"))
				.WillCascadeOnDelete(true);

		}


        public static void RunMigrations()
        {
            var config = new Core.Migrations.Configuration();
            var migrator = new DbMigrator(config);
            migrator.Update(); //when debugging in VS exception will be throw on this point. Just comment out this line. http://stackoverflow.com/questions/11979026/entity-framework-5-expects-createdon-column-from-migrationhistory-table
        }

		//FbAccount module
		public DbSet<FbAccount> FbAccounts { get; set; }
		public DbSet<FbAccountSettings> FbAccountSettings { get; set; }
		public DbSet<TestRailConfiguration> TestRailConfigurations { get; set; }

		//Log module        
		public DbSet<LogEntry> LogEntries { get; set; }
		public DbSet<LogEntryTypeLookup> LogEntryTypes { get; set; }
		public DbSet<LogEntityTypeLookup> LogEntityTypes { get; set; }
	}
}

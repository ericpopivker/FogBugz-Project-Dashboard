namespace FogBugzPd.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeId = c.Int(nullable: false),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LogEntryType", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.LogEntryType",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        EntityType = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LogEntityType", t => t.EntityTypeId, cascadeDelete: true)
                .Index(t => t.EntityTypeId);
            
            CreateTable(
                "dbo.LogEntityType",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.LogEntryType", new[] { "EntityTypeId" });
            DropIndex("dbo.LogEntry", new[] { "TypeId" });
            DropForeignKey("dbo.LogEntryType", "EntityTypeId", "dbo.LogEntityType");
            DropForeignKey("dbo.LogEntry", "TypeId", "dbo.LogEntryType");
            DropTable("dbo.LogEntityType");
            DropTable("dbo.LogEntryType");
            DropTable("dbo.LogEntry");
        }
    }
}

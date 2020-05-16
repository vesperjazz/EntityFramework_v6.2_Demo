namespace EntityFrameworkDemo.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gender",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 200),
                        LastName = c.String(nullable: false, maxLength: 200),
                        DateOfBirth = c.DateTime(),
                        GenderID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gender", t => t.GenderID)
                .Index(t => t.GenderID);
            
            CreateTable(
                "dbo.PersonContactNumber",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        PhoneNumber = c.String(nullable: false, maxLength: 100),
                        PersonID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Person", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.PhoneNumber, unique: true)
                .Index(t => t.PersonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonContactNumber", "PersonID", "dbo.Person");
            DropForeignKey("dbo.Person", "GenderID", "dbo.Gender");
            DropIndex("dbo.PersonContactNumber", new[] { "PersonID" });
            DropIndex("dbo.PersonContactNumber", new[] { "PhoneNumber" });
            DropIndex("dbo.Person", new[] { "GenderID" });
            DropTable("dbo.PersonContactNumber");
            DropTable("dbo.Person");
            DropTable("dbo.Gender");
        }
    }
}

namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CarYear = c.Int(nullable: false),
                        Registration = c.String(),
                        TaxiNumber = c.Int(nullable: false),
                        CarType = c.Int(nullable: false),
                        DriverId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.DriverId)
                .Index(t => t.DriverId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CooridateX = c.Int(nullable: false),
                        CooridateY = c.Int(nullable: false),
                        Street = c.String(),
                        HouseNumber = c.String(),
                        City = c.String(),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Grade = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        DriveId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Drives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CarType = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        Status = c.Int(nullable: false),
                        OriginId = c.Int(),
                        DestinationId = c.Int(),
                        AdminId = c.Int(),
                        DriverId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.AdminId)
                .ForeignKey("dbo.Locations", t => t.DestinationId)
                .ForeignKey("dbo.AppUsers", t => t.DriverId)
                .ForeignKey("dbo.Locations", t => t.OriginId)
                .Index(t => t.OriginId)
                .Index(t => t.DestinationId)
                .Index(t => t.AdminId)
                .Index(t => t.DriverId);
            
            AddColumn("dbo.AppUsers", "Username", c => c.String());
            AddColumn("dbo.AppUsers", "SSN", c => c.String());
            AddColumn("dbo.AppUsers", "Gender", c => c.Int());
            AddColumn("dbo.AppUsers", "ContactNumber", c => c.String());
            AddColumn("dbo.AppUsers", "Role", c => c.String());
            AddColumn("dbo.AppUsers", "DriveType", c => c.Int());
            AddColumn("dbo.AppUsers", "Email", c => c.String());
            AddColumn("dbo.AppUsers", "CarId", c => c.Int());
            AddColumn("dbo.AppUsers", "LocationId", c => c.Int());
            AddColumn("dbo.AppUsers", "PasswordLogin", c => c.String());
            AddColumn("dbo.AppUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AppUsers", "CarId");
            CreateIndex("dbo.AppUsers", "LocationId");
            AddForeignKey("dbo.AppUsers", "CarId", "dbo.Cars", "Id");
            AddForeignKey("dbo.AppUsers", "LocationId", "dbo.Locations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drives", "OriginId", "dbo.Locations");
            DropForeignKey("dbo.Drives", "DriverId", "dbo.AppUsers");
            DropForeignKey("dbo.Drives", "DestinationId", "dbo.Locations");
            DropForeignKey("dbo.Drives", "AdminId", "dbo.AppUsers");
            DropForeignKey("dbo.AppUsers", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.AppUsers", "CarId", "dbo.Cars");
            DropForeignKey("dbo.Cars", "DriverId", "dbo.AppUsers");
            DropIndex("dbo.Drives", new[] { "DriverId" });
            DropIndex("dbo.Drives", new[] { "AdminId" });
            DropIndex("dbo.Drives", new[] { "DestinationId" });
            DropIndex("dbo.Drives", new[] { "OriginId" });
            DropIndex("dbo.Cars", new[] { "DriverId" });
            DropIndex("dbo.AppUsers", new[] { "LocationId" });
            DropIndex("dbo.AppUsers", new[] { "CarId" });
            DropColumn("dbo.AppUsers", "Discriminator");
            DropColumn("dbo.AppUsers", "PasswordLogin");
            DropColumn("dbo.AppUsers", "LocationId");
            DropColumn("dbo.AppUsers", "CarId");
            DropColumn("dbo.AppUsers", "Email");
            DropColumn("dbo.AppUsers", "DriveType");
            DropColumn("dbo.AppUsers", "Role");
            DropColumn("dbo.AppUsers", "ContactNumber");
            DropColumn("dbo.AppUsers", "Gender");
            DropColumn("dbo.AppUsers", "SSN");
            DropColumn("dbo.AppUsers", "Username");
            DropTable("dbo.Drives");
            DropTable("dbo.Comments");
            DropTable("dbo.Locations");
            DropTable("dbo.Cars");
        }
    }
}

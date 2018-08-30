namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateall : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Cars", new[] { "CarType" });
            AlterColumn("dbo.AppUsers", "Gender", c => c.String());
            AlterColumn("dbo.AppUsers", "DriveType", c => c.String());
            AlterColumn("dbo.Drives", "CarType", c => c.String());
            AlterColumn("dbo.Drives", "Status", c => c.String());
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Cars", "CarType");
            AlterColumn("dbo.Drives", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.Drives", "CarType", c => c.Int(nullable: false));
            AlterColumn("dbo.Cars", "CarType", c => c.Int(nullable: false));
            AlterColumn("dbo.AppUsers", "DriveType", c => c.Int());
            AlterColumn("dbo.AppUsers", "Gender", c => c.Int());
        }
    }
}

namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class availabledriver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Available", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Available");
        }
    }
}

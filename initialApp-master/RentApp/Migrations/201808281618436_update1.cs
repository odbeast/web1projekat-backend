namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Drives", "CommentId", c => c.Int());
            CreateIndex("dbo.Drives", "CommentId");
            AddForeignKey("dbo.Drives", "CommentId", "dbo.Comments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drives", "CommentId", "dbo.Comments");
            DropIndex("dbo.Drives", new[] { "CommentId" });
            DropColumn("dbo.Drives", "CommentId");
        }
    }
}

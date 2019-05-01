namespace lec0Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserDataToProductAndCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Categories", "UserId");
            CreateIndex("dbo.Products", "UserId");
            AddForeignKey("dbo.Categories", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Categories", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Products", new[] { "UserId" });
            DropIndex("dbo.Categories", new[] { "UserId" });
            DropColumn("dbo.Products", "UserId");
            DropColumn("dbo.Categories", "UserId");
        }
    }
}

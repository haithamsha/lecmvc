namespace lec0Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Product_AddCreationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Name", c => c.String());
            DropColumn("dbo.Products", "CreationDate");
        }
    }
}

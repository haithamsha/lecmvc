namespace lec0Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Product_MakeCreationDateNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "CreationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "CreationDate", c => c.DateTime(nullable: false));
        }
    }
}

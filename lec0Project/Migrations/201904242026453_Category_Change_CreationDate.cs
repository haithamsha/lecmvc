namespace lec0Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Category_Change_CreationDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Categories", "CreationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Categories", "Name", c => c.String());
        }
    }
}

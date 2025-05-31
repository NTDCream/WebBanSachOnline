namespace WebBanSachOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAttributeDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Book", "author", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Book", "author");
        }
    }
}

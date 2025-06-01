namespace WebBanSachOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUniqueIndexOnReview : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Review", "IX_User_Book");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Review", new[] { "userId", "bookId" }, unique: true, name: "IX_User_Book");
        }
    }
}

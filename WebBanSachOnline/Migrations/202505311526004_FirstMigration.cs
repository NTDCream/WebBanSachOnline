namespace WebBanSachOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false, maxLength: 512),
                        slug = c.String(nullable: false, maxLength: 512),
                        categoryId = c.Int(nullable: false),
                        description = c.String(maxLength: 512),
                        image = c.String(maxLength: 512),
                        price = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        soldQuantity = c.Int(nullable: false),
                        originalPrice = c.Int(nullable: false),
                        rate = c.Double(),
                        reviewCount = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Category", t => t.categoryId)
                .Index(t => t.categoryId);
            
            CreateTable(
                "dbo.CartItem",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        bookId = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.userId)
                .ForeignKey("dbo.Book", t => t.bookId)
                .Index(t => t.userId)
                .Index(t => t.bookId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fullName = c.String(nullable: false, maxLength: 255),
                        username = c.String(nullable: false, maxLength: 255),
                        email = c.String(nullable: false, maxLength: 255),
                        phone = c.String(maxLength: 20),
                        address = c.String(nullable: false, maxLength: 512),
                        password = c.String(nullable: false, maxLength: 255),
                        role = c.String(),
                        isActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        slug = c.String(nullable: false, maxLength: 50),
                        status = c.String(nullable: false, maxLength: 20),
                        totalAmount = c.Decimal(nullable: false, precision: 12, scale: 2),
                        customerName = c.String(nullable: false, maxLength: 255),
                        phone = c.String(nullable: false, maxLength: 20),
                        address = c.String(nullable: false, maxLength: 512),
                        price = c.Int(),
                        paymentMethod = c.String(maxLength: 100),
                        createdDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.userId)
                .Index(t => t.userId);
            
            CreateTable(
                "dbo.OrderDetail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        bookId = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Order", t => t.orderId)
                .ForeignKey("dbo.Book", t => t.bookId)
                .Index(t => t.orderId)
                .Index(t => t.bookId);
            
            CreateTable(
                "dbo.Review",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        bookId = c.Int(nullable: false),
                        rate = c.Int(nullable: false),
                        comment = c.String(maxLength: 512),
                        createdDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.userId)
                .ForeignKey("dbo.Book", t => t.bookId)
                .Index(t => t.userId)
                .Index(t => t.bookId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false, maxLength: 255),
                        slug = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Book_Author",
                c => new
                    {
                        authorId = c.Int(nullable: false),
                        bookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.authorId, t.bookId })
                .ForeignKey("dbo.Author", t => t.authorId, cascadeDelete: true)
                .ForeignKey("dbo.Book", t => t.bookId, cascadeDelete: true)
                .Index(t => t.authorId)
                .Index(t => t.bookId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Book_Author", "bookId", "dbo.Book");
            DropForeignKey("dbo.Book_Author", "authorId", "dbo.Author");
            DropForeignKey("dbo.Review", "bookId", "dbo.Book");
            DropForeignKey("dbo.OrderDetail", "bookId", "dbo.Book");
            DropForeignKey("dbo.Book", "categoryId", "dbo.Category");
            DropForeignKey("dbo.CartItem", "bookId", "dbo.Book");
            DropForeignKey("dbo.Review", "userId", "dbo.User");
            DropForeignKey("dbo.Order", "userId", "dbo.User");
            DropForeignKey("dbo.OrderDetail", "orderId", "dbo.Order");
            DropForeignKey("dbo.CartItem", "userId", "dbo.User");
            DropIndex("dbo.Book_Author", new[] { "bookId" });
            DropIndex("dbo.Book_Author", new[] { "authorId" });
            DropIndex("dbo.Review", new[] { "bookId" });
            DropIndex("dbo.Review", new[] { "userId" });
            DropIndex("dbo.OrderDetail", new[] { "bookId" });
            DropIndex("dbo.OrderDetail", new[] { "orderId" });
            DropIndex("dbo.Order", new[] { "userId" });
            DropIndex("dbo.CartItem", new[] { "bookId" });
            DropIndex("dbo.CartItem", new[] { "userId" });
            DropIndex("dbo.Book", new[] { "categoryId" });
            DropTable("dbo.Book_Author");
            DropTable("dbo.Category");
            DropTable("dbo.Review");
            DropTable("dbo.OrderDetail");
            DropTable("dbo.Order");
            DropTable("dbo.User");
            DropTable("dbo.CartItem");
            DropTable("dbo.Book");
            DropTable("dbo.Author");
        }
    }
}

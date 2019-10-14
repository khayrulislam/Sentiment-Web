namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeuser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Repository", "UserId", "dbo.User");
            DropIndex("dbo.Repository", new[] { "UserId" });
            DropColumn("dbo.Repository", "UserId");
            DropTable("dbo.User");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Repository", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Repository", "UserId");
            AddForeignKey("dbo.Repository", "UserId", "dbo.User", "Id", cascadeDelete: true);
        }
    }
}

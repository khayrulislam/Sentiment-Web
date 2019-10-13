namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removecontribution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issue", "RepostoryId", c => c.Int(nullable: false));
            AddColumn("dbo.Issue", "IssueNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Issue", "Repository_Id", c => c.Int());
            CreateIndex("dbo.Issue", "Repository_Id");
            AddForeignKey("dbo.Issue", "Repository_Id", "dbo.Repository", "Id");
            DropColumn("dbo.Contributor", "Contribution");
            DropColumn("dbo.Issue", "IssueId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Issue", "IssueId", c => c.Int(nullable: false));
            AddColumn("dbo.Contributor", "Contribution", c => c.Int(nullable: false));
            DropForeignKey("dbo.Issue", "Repository_Id", "dbo.Repository");
            DropIndex("dbo.Issue", new[] { "Repository_Id" });
            DropColumn("dbo.Issue", "Repository_Id");
            DropColumn("dbo.Issue", "IssueNumber");
            DropColumn("dbo.Issue", "RepostoryId");
        }
    }
}

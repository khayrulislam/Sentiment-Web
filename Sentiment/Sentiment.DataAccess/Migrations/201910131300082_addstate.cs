namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Issue", "Repository_Id", "dbo.Repository");
            DropIndex("dbo.Issue", new[] { "Repository_Id" });
            RenameColumn(table: "dbo.Issue", name: "Repository_Id", newName: "RepositoryId");
            AddColumn("dbo.Issue", "State", c => c.String());
            AddColumn("dbo.PullRequest", "State", c => c.String());
            AlterColumn("dbo.Issue", "RepositoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Issue", "RepositoryId");
            AddForeignKey("dbo.Issue", "RepositoryId", "dbo.Repository", "Id", cascadeDelete: true);
            DropColumn("dbo.Issue", "RepostoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Issue", "RepostoryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Issue", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.Issue", new[] { "RepositoryId" });
            AlterColumn("dbo.Issue", "RepositoryId", c => c.Int());
            DropColumn("dbo.PullRequest", "State");
            DropColumn("dbo.Issue", "State");
            RenameColumn(table: "dbo.Issue", name: "RepositoryId", newName: "Repository_Id");
            CreateIndex("dbo.Issue", "Repository_Id");
            AddForeignKey("dbo.Issue", "Repository_Id", "dbo.Repository", "Id");
        }
    }
}

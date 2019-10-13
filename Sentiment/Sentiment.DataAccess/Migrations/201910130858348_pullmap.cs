namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pullmap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PullRequest", "RepositoryId", c => c.Int(nullable: false));
            AlterColumn("dbo.Repository", "RepoId", c => c.Long(nullable: false));
            CreateIndex("dbo.PullRequest", "RepositoryId");
            AddForeignKey("dbo.PullRequest", "RepositoryId", "dbo.Repository", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PullRequest", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.PullRequest", new[] { "RepositoryId" });
            AlterColumn("dbo.Repository", "RepoId", c => c.Int(nullable: false));
            DropColumn("dbo.PullRequest", "RepositoryId");
        }
    }
}

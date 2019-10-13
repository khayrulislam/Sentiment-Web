namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BranchCommit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        CommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Commit", t => t.CommitId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.CommitId);
            
            CreateTable(
                "dbo.Branch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Sha = c.String(),
                        RepositoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryId);
            
            CreateTable(
                "dbo.Repository",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepoId = c.Long(nullable: false),
                        Name = c.String(),
                        OwnerName = c.String(),
                        Url = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RepositoryContributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        ContributorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.ContributorId, cascadeDelete: true)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.ContributorId);
            
            CreateTable(
                "dbo.Contributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateTable(
                "dbo.Commit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Sha = c.String(),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        WriterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.WriterId, cascadeDelete: true)
                .Index(t => t.WriterId);
            
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        WriterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.WriterId, cascadeDelete: true)
                .Index(t => t.WriterId);
            
            CreateTable(
                "dbo.Issue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        IssueNumber = c.Int(nullable: false),
                        Title = c.String(),
                        State = c.String(),
                        IssueId = c.Long(nullable: false),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        WriterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .ForeignKey("dbo.Contributor", t => t.WriterId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.WriterId);
            
            CreateTable(
                "dbo.PullRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestNumber = c.Int(nullable: false),
                        Title = c.String(),
                        RepositoryId = c.Int(nullable: false),
                        State = c.String(),
                        PullRequestId = c.Long(nullable: false),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        WriterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .ForeignKey("dbo.Contributor", t => t.WriterId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.WriterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PullRequest", "WriterId", "dbo.Contributor");
            DropForeignKey("dbo.PullRequest", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Issue", "WriterId", "dbo.Contributor");
            DropForeignKey("dbo.Issue", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Comment", "WriterId", "dbo.Contributor");
            DropForeignKey("dbo.BranchCommit", "CommitId", "dbo.Commit");
            DropForeignKey("dbo.Commit", "WriterId", "dbo.Contributor");
            DropForeignKey("dbo.BranchCommit", "BranchId", "dbo.Branch");
            DropForeignKey("dbo.Repository", "UserId", "dbo.User");
            DropForeignKey("dbo.RepositoryContributor", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributor", "ContributorId", "dbo.Contributor");
            DropForeignKey("dbo.Branch", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.PullRequest", new[] { "WriterId" });
            DropIndex("dbo.PullRequest", new[] { "RepositoryId" });
            DropIndex("dbo.Issue", new[] { "WriterId" });
            DropIndex("dbo.Issue", new[] { "RepositoryId" });
            DropIndex("dbo.Comment", new[] { "WriterId" });
            DropIndex("dbo.Commit", new[] { "WriterId" });
            DropIndex("dbo.RepositoryContributor", new[] { "ContributorId" });
            DropIndex("dbo.RepositoryContributor", new[] { "RepositoryId" });
            DropIndex("dbo.Repository", new[] { "UserId" });
            DropIndex("dbo.Branch", new[] { "RepositoryId" });
            DropIndex("dbo.BranchCommit", new[] { "CommitId" });
            DropIndex("dbo.BranchCommit", new[] { "BranchId" });
            DropTable("dbo.PullRequest");
            DropTable("dbo.Issue");
            DropTable("dbo.Comment");
            DropTable("dbo.Commit");
            DropTable("dbo.User");
            DropTable("dbo.Contributor");
            DropTable("dbo.RepositoryContributor");
            DropTable("dbo.Repository");
            DropTable("dbo.Branch");
            DropTable("dbo.BranchCommit");
        }
    }
}
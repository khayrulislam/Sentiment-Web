namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create : DbMigration
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
                        AnalysisDate = c.DateTimeOffset(nullable: false, precision: 7),
                        State = c.Int(nullable: false),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Commit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sha = c.String(),
                        RepositoryId = c.Int(),
                        DateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Message = c.String(),
                        Pos = c.Int(nullable: false),
                        Neg = c.Int(nullable: false),
                        CreatorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.CreatorId)
                .ForeignKey("dbo.Repository", t => t.RepositoryId)
                .Index(t => t.RepositoryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.CommitComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommitId = c.Int(nullable: false),
                        CommentNumber = c.Long(nullable: false),
                        Date = c.DateTimeOffset(precision: 7),
                        Message = c.String(),
                        Pos = c.Int(nullable: false),
                        Neg = c.Int(nullable: false),
                        CreatorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Commit", t => t.CommitId, cascadeDelete: true)
                .ForeignKey("dbo.Contributor", t => t.CreatorId)
                .Index(t => t.CommitId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Contributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ContributorId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IssueComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        RepositoryId = c.Int(nullable: false),
                        CommentNumber = c.Long(nullable: false),
                        Date = c.DateTimeOffset(precision: 7),
                        Message = c.String(),
                        Pos = c.Int(nullable: false),
                        Neg = c.Int(nullable: false),
                        CreatorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.CreatorId)
                .ForeignKey("dbo.Issue", t => t.IssueId, cascadeDelete: true)
                .Index(t => t.IssueId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Issue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        IssueNumber = c.Int(nullable: false),
                        PosTitle = c.Int(nullable: false),
                        NegTitle = c.Int(nullable: false),
                        State = c.String(),
                        IssueType = c.Int(nullable: false),
                        CreateDate = c.DateTimeOffset(precision: 7),
                        UpdateDate = c.DateTimeOffset(precision: 7),
                        CloseDate = c.DateTimeOffset(precision: 7),
                        Title = c.String(),
                        Body = c.String(),
                        Lables = c.String(),
                        Assignees = c.String(),
                        Merged = c.Boolean(nullable: false),
                        MergeDate = c.DateTimeOffset(precision: 7),
                        Pos = c.Int(nullable: false),
                        Neg = c.Int(nullable: false),
                        CreatorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.CreatorId)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.PullCommit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommitSha = c.String(),
                        PullRequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Issue", t => t.PullRequestId, cascadeDelete: true)
                .Index(t => t.PullRequestId);
            
            CreateTable(
                "dbo.RepositoryContributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        ContributorId = c.Int(nullable: false),
                        Contribution = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.ContributorId, cascadeDelete: true)
                .ForeignKey("dbo.Repository", t => t.RepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryId)
                .Index(t => t.ContributorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BranchCommit", "CommitId", "dbo.Commit");
            DropForeignKey("dbo.BranchCommit", "BranchId", "dbo.Branch");
            DropForeignKey("dbo.Commit", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributor", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributor", "ContributorId", "dbo.Contributor");
            DropForeignKey("dbo.Issue", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Issue", "CreatorId", "dbo.Contributor");
            DropForeignKey("dbo.PullCommit", "PullRequestId", "dbo.Issue");
            DropForeignKey("dbo.IssueComment", "IssueId", "dbo.Issue");
            DropForeignKey("dbo.IssueComment", "CreatorId", "dbo.Contributor");
            DropForeignKey("dbo.Commit", "CreatorId", "dbo.Contributor");
            DropForeignKey("dbo.CommitComment", "CreatorId", "dbo.Contributor");
            DropForeignKey("dbo.CommitComment", "CommitId", "dbo.Commit");
            DropForeignKey("dbo.Branch", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.RepositoryContributor", new[] { "ContributorId" });
            DropIndex("dbo.RepositoryContributor", new[] { "RepositoryId" });
            DropIndex("dbo.PullCommit", new[] { "PullRequestId" });
            DropIndex("dbo.Issue", new[] { "CreatorId" });
            DropIndex("dbo.Issue", new[] { "RepositoryId" });
            DropIndex("dbo.IssueComment", new[] { "CreatorId" });
            DropIndex("dbo.IssueComment", new[] { "IssueId" });
            DropIndex("dbo.CommitComment", new[] { "CreatorId" });
            DropIndex("dbo.CommitComment", new[] { "CommitId" });
            DropIndex("dbo.Commit", new[] { "CreatorId" });
            DropIndex("dbo.Commit", new[] { "RepositoryId" });
            DropIndex("dbo.Branch", new[] { "RepositoryId" });
            DropIndex("dbo.BranchCommit", new[] { "CommitId" });
            DropIndex("dbo.BranchCommit", new[] { "BranchId" });
            DropTable("dbo.RepositoryContributor");
            DropTable("dbo.PullCommit");
            DropTable("dbo.Issue");
            DropTable("dbo.IssueComment");
            DropTable("dbo.Contributor");
            DropTable("dbo.CommitComment");
            DropTable("dbo.Commit");
            DropTable("dbo.Repository");
            DropTable("dbo.Branch");
            DropTable("dbo.BranchCommit");
        }
    }
}

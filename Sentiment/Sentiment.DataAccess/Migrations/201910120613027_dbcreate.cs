namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbcreate : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Commit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommiterId = c.Int(nullable: false),
                        Message = c.String(),
                        Sha = c.String(),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.CommiterId, cascadeDelete: true)
                .Index(t => t.CommiterId);
            
            CreateTable(
                "dbo.Contributor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Contribution = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.Repository",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OwnerName = c.String(),
                        Url = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BranchCommit", "CommitId", "dbo.Commit");
            DropForeignKey("dbo.Commit", "CommiterId", "dbo.Contributor");
            DropForeignKey("dbo.RepositoryContributor", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.Repository", "UserId", "dbo.User");
            DropForeignKey("dbo.Branch", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributor", "ContributorId", "dbo.Contributor");
            DropForeignKey("dbo.BranchCommit", "BranchId", "dbo.Branch");
            DropIndex("dbo.Repository", new[] { "UserId" });
            DropIndex("dbo.RepositoryContributor", new[] { "ContributorId" });
            DropIndex("dbo.RepositoryContributor", new[] { "RepositoryId" });
            DropIndex("dbo.Commit", new[] { "CommiterId" });
            DropIndex("dbo.BranchCommit", new[] { "CommitId" });
            DropIndex("dbo.BranchCommit", new[] { "BranchId" });
            DropIndex("dbo.Branch", new[] { "RepositoryId" });
            DropTable("dbo.User");
            DropTable("dbo.Repository");
            DropTable("dbo.RepositoryContributor");
            DropTable("dbo.Contributor");
            DropTable("dbo.Commit");
            DropTable("dbo.BranchCommit");
            DropTable("dbo.Branch");
        }
    }
}

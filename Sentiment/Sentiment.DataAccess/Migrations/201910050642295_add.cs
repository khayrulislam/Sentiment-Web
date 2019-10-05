namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add : DbMigration
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
                "dbo.RepositoryContributors",
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
                        Contribution = c.Int(nullable: false),
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
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        Branch_Id = c.Int(),
                        Commiter_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branch", t => t.Branch_Id)
                .ForeignKey("dbo.Contributor", t => t.Commiter_Id)
                .Index(t => t.Branch_Id)
                .Index(t => t.Commiter_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Commit", "Commiter_Id", "dbo.Contributor");
            DropForeignKey("dbo.Commit", "Branch_Id", "dbo.Branch");
            DropForeignKey("dbo.Repository", "UserId", "dbo.User");
            DropForeignKey("dbo.RepositoryContributors", "RepositoryId", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributors", "ContributorId", "dbo.Contributor");
            DropForeignKey("dbo.Branch", "RepositoryId", "dbo.Repository");
            DropIndex("dbo.Commit", new[] { "Commiter_Id" });
            DropIndex("dbo.Commit", new[] { "Branch_Id" });
            DropIndex("dbo.RepositoryContributors", new[] { "ContributorId" });
            DropIndex("dbo.RepositoryContributors", new[] { "RepositoryId" });
            DropIndex("dbo.Repository", new[] { "UserId" });
            DropIndex("dbo.Branch", new[] { "RepositoryId" });
            DropTable("dbo.Commit");
            DropTable("dbo.User");
            DropTable("dbo.Contributor");
            DropTable("dbo.RepositoryContributors");
            DropTable("dbo.Repository");
            DropTable("dbo.Branch");
        }
    }
}

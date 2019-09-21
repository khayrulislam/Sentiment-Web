namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Sha = c.String(),
                        RepositoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repositories", t => t.RepositoryId, cascadeDelete: true)
                .Index(t => t.RepositoryId);
            
            CreateTable(
                "dbo.Repositories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OwnerName = c.String(),
                        Url = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Contributors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Contribution = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
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
                "dbo.Commits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PosSentiment = c.Int(nullable: false),
                        NegSentiment = c.Int(nullable: false),
                        Branch_Id = c.Int(),
                        Commiter_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.Branch_Id)
                .ForeignKey("dbo.Contributors", t => t.Commiter_Id)
                .Index(t => t.Branch_Id)
                .Index(t => t.Commiter_Id);
            
            CreateTable(
                "dbo.ContributorRepositories",
                c => new
                    {
                        Contributor_Id = c.Int(nullable: false),
                        Repository_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Contributor_Id, t.Repository_Id })
                .ForeignKey("dbo.Contributors", t => t.Contributor_Id, cascadeDelete: true)
                .ForeignKey("dbo.Repositories", t => t.Repository_Id, cascadeDelete: true)
                .Index(t => t.Contributor_Id)
                .Index(t => t.Repository_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Commits", "Commiter_Id", "dbo.Contributors");
            DropForeignKey("dbo.Commits", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Repositories", "UserId", "dbo.Users");
            DropForeignKey("dbo.ContributorRepositories", "Repository_Id", "dbo.Repositories");
            DropForeignKey("dbo.ContributorRepositories", "Contributor_Id", "dbo.Contributors");
            DropForeignKey("dbo.Branches", "RepositoryId", "dbo.Repositories");
            DropIndex("dbo.ContributorRepositories", new[] { "Repository_Id" });
            DropIndex("dbo.ContributorRepositories", new[] { "Contributor_Id" });
            DropIndex("dbo.Commits", new[] { "Commiter_Id" });
            DropIndex("dbo.Commits", new[] { "Branch_Id" });
            DropIndex("dbo.Repositories", new[] { "UserId" });
            DropIndex("dbo.Branches", new[] { "RepositoryId" });
            DropTable("dbo.ContributorRepositories");
            DropTable("dbo.Commits");
            DropTable("dbo.Users");
            DropTable("dbo.Contributors");
            DropTable("dbo.Repositories");
            DropTable("dbo.Branches");
        }
    }
}

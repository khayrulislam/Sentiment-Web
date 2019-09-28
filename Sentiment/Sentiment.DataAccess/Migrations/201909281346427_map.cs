namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContributorDataRepositoryDatas", "ContributorData_Id", "dbo.Contributor");
            DropForeignKey("dbo.ContributorDataRepositoryDatas", "RepositoryData_Id", "dbo.Repository");
            DropIndex("dbo.ContributorDataRepositoryDatas", new[] { "ContributorData_Id" });
            DropIndex("dbo.ContributorDataRepositoryDatas", new[] { "RepositoryData_Id" });
            CreateTable(
                "dbo.RepositoryContributorsMap",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepositoryId = c.Int(nullable: false),
                        ContributorId = c.Int(nullable: false),
                        ContributorData_Id = c.Int(),
                        RepositoryData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contributor", t => t.ContributorData_Id)
                .ForeignKey("dbo.Repository", t => t.RepositoryData_Id)
                .Index(t => t.ContributorData_Id)
                .Index(t => t.RepositoryData_Id);
            
            DropTable("dbo.ContributorDataRepositoryDatas");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ContributorDataRepositoryDatas",
                c => new
                    {
                        ContributorData_Id = c.Int(nullable: false),
                        RepositoryData_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContributorData_Id, t.RepositoryData_Id });
            
            DropForeignKey("dbo.RepositoryContributorsMap", "RepositoryData_Id", "dbo.Repository");
            DropForeignKey("dbo.RepositoryContributorsMap", "ContributorData_Id", "dbo.Contributor");
            DropIndex("dbo.RepositoryContributorsMap", new[] { "RepositoryData_Id" });
            DropIndex("dbo.RepositoryContributorsMap", new[] { "ContributorData_Id" });
            DropTable("dbo.RepositoryContributorsMap");
            CreateIndex("dbo.ContributorDataRepositoryDatas", "RepositoryData_Id");
            CreateIndex("dbo.ContributorDataRepositoryDatas", "ContributorData_Id");
            AddForeignKey("dbo.ContributorDataRepositoryDatas", "RepositoryData_Id", "dbo.Repository", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ContributorDataRepositoryDatas", "ContributorData_Id", "dbo.Contributor", "Id", cascadeDelete: true);
        }
    }
}

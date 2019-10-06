namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commit_column_add : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Commit", "Branch_Id", "dbo.Branch");
            DropForeignKey("dbo.Commit", "Commiter_Id", "dbo.Contributor");
            DropIndex("dbo.Commit", new[] { "Branch_Id" });
            DropIndex("dbo.Commit", new[] { "Commiter_Id" });
            RenameColumn(table: "dbo.Commit", name: "Branch_Id", newName: "BranchId");
            RenameColumn(table: "dbo.Commit", name: "Commiter_Id", newName: "CommiterId");
            AddColumn("dbo.Commit", "Message", c => c.String());
            AddColumn("dbo.Commit", "Sha", c => c.String());
            AlterColumn("dbo.Commit", "BranchId", c => c.Int(nullable: false));
            AlterColumn("dbo.Commit", "CommiterId", c => c.Int(nullable: false));
            CreateIndex("dbo.Commit", "CommiterId");
            CreateIndex("dbo.Commit", "BranchId");
            AddForeignKey("dbo.Commit", "BranchId", "dbo.Branch", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Commit", "CommiterId", "dbo.Contributor", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Commit", "CommiterId", "dbo.Contributor");
            DropForeignKey("dbo.Commit", "BranchId", "dbo.Branch");
            DropIndex("dbo.Commit", new[] { "BranchId" });
            DropIndex("dbo.Commit", new[] { "CommiterId" });
            AlterColumn("dbo.Commit", "CommiterId", c => c.Int());
            AlterColumn("dbo.Commit", "BranchId", c => c.Int());
            DropColumn("dbo.Commit", "Sha");
            DropColumn("dbo.Commit", "Message");
            RenameColumn(table: "dbo.Commit", name: "CommiterId", newName: "Commiter_Id");
            RenameColumn(table: "dbo.Commit", name: "BranchId", newName: "Branch_Id");
            CreateIndex("dbo.Commit", "Commiter_Id");
            CreateIndex("dbo.Commit", "Branch_Id");
            AddForeignKey("dbo.Commit", "Commiter_Id", "dbo.Contributor", "Id");
            AddForeignKey("dbo.Commit", "Branch_Id", "dbo.Branch", "Id");
        }
    }
}

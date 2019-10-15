namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repoi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Commit", "RepositoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Commit", "RepositoryId");
        }
    }
}

namespace Sentiment.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfirstname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FirstName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "FirstName");
        }
    }
}

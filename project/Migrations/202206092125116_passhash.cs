namespace project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class passhash : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "passwordenc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "passwordenc");
        }
    }
}

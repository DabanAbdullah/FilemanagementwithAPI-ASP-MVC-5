namespace project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customfieldsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "profilephoto", c => c.String());
            AddColumn("dbo.AspNetUsers", "fullname", c => c.String());
            AddColumn("dbo.AspNetUsers", "uid", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "uid");
            DropColumn("dbo.AspNetUsers", "fullname");
            DropColumn("dbo.AspNetUsers", "profilephoto");
        }
    }
}

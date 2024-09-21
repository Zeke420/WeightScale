namespace WeightScale.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Packages", "FullPackageDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.Packages", "EmptyPackageDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Packages", "EmptyPackageDate");
            DropColumn("dbo.Packages", "FullPackageDate");
        }
    }
}

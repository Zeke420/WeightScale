namespace WeightScale.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageWeightNulls : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Packages", "FullWeight", c => c.Double());
            AlterColumn("dbo.Packages", "EmptyWeight", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Packages", "EmptyWeight", c => c.Double(nullable: false));
            AlterColumn("dbo.Packages", "FullWeight", c => c.Double(nullable: false));
        }
    }
}

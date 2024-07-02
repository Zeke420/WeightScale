namespace WeightScale.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Couriers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShipmentDate = c.DateTime(nullable: false),
                        CourierId = c.Int(nullable: false),
                        TotalWeight = c.Double(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Couriers", t => t.CourierId, cascadeDelete: true)
                .Index(t => t.CourierId);
            
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullWeight = c.Double(nullable: false),
                        EmptyWeight = c.Double(nullable: false),
                        ShipmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId, cascadeDelete: true)
                .Index(t => t.ShipmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shipments", "CourierId", "dbo.Couriers");
            DropForeignKey("dbo.Packages", "ShipmentId", "dbo.Shipments");
            DropIndex("dbo.Packages", new[] { "ShipmentId" });
            DropIndex("dbo.Shipments", new[] { "CourierId" });
            DropTable("dbo.Packages");
            DropTable("dbo.Shipments");
            DropTable("dbo.Couriers");
        }
    }
}

namespace MovingCars.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DriverId", c => c.Int());
            CreateIndex("dbo.Orders", "DriverId");
            AddForeignKey("dbo.Orders", "DriverId", "dbo.Drivers", "Id");
            DropColumn("dbo.Orders", "Driver");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Driver", c => c.String(maxLength: 255));
            DropForeignKey("dbo.Orders", "DriverId", "dbo.Drivers");
            DropIndex("dbo.Orders", new[] { "DriverId" });
            DropColumn("dbo.Orders", "DriverId");
        }
    }
}

namespace MovingCars.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdPassenger : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Passengers", "Phone", c => c.String(maxLength: 100));
            AlterColumn("dbo.Passengers", "Department", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Passengers", "Department", c => c.String(maxLength: 255));
            AlterColumn("dbo.Passengers", "Phone", c => c.String(maxLength: 20));
        }
    }
}

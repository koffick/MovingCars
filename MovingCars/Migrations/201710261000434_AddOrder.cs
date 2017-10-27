namespace MovingCars.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        StartAddress = c.String(maxLength: 255),
                        EndAddress = c.String(maxLength: 255),
                        OutOfTown = c.Byte(nullable: false),
                        Passenger = c.String(maxLength: 255),
                        Driver = c.String(maxLength: 255),
                        Note = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Orders");
        }
    }
}

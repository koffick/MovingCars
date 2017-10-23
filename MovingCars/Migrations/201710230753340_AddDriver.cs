namespace MovingCars.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDriver : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                        Patronymic = c.String(maxLength: 255),
                        Call = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Drivers");
        }
    }
}

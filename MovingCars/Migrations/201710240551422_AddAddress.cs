namespace MovingCars.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Region = c.String(maxLength: 255),
                        District = c.String(maxLength: 255),
                        City = c.String(maxLength: 255),
                        Locality = c.String(maxLength: 255),
                        Street = c.String(maxLength: 255),
                        HouseNumber = c.String(maxLength: 255),
                        Additional = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Addresses");
        }
    }
}

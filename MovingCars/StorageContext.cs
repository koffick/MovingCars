using MovingCars.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MovingCars
{
    public class StorageContext : DbContext
    {
        public StorageContext()
            : base("name=DefaultConnection")
        { }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
    }
}
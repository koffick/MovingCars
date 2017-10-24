using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; }

        [StringLength(255)]
        public string Region { get; set; }
        [StringLength(255)]
        public string District { get; set; }
        [StringLength(255)]
        public string City { get; set; }
        [StringLength(255)]
        public string Locality { get; set; }
        [StringLength(255)]
        public string Street { get; set; }
        [StringLength(255)]
        public string HouseNumber { get; set; }
        [StringLength(255)]
        public string Additional { get; set; }

    }
}
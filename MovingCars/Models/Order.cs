using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(255)]
        public string StartAddress { get; set; }
        [StringLength(255)]
        public string EndAddress { get; set; }
        public byte OutOfTown { get; set; }
        [StringLength(255)]
        public string Passenger { get; set; }
        [StringLength(255)]
        public string Driver { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }

    }
}
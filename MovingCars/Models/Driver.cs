using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Patronymic { get; set; }
        [StringLength(255)]
        public string Call { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }

    }
}
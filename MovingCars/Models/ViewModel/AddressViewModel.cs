using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models.ViewModel
{
    public class AddressViewModel
    {
        public string Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Регион")]
        [Required(ErrorMessage = "Регион - обязателен для ввода")]
        public string Region { get; set; }
        [StringLength(255)]
        [Display(Name = "Район")]
        public string District { get; set; }
        [StringLength(255)]
        [Display(Name = "Город")]
        public string City { get; set; }
        [StringLength(255)]
        [Display(Name = "Нас.пункт")]
        public string Locality { get; set; }
        [StringLength(255)]
        [Display(Name = "Улица")]
        public string Street { get; set; }
        [StringLength(255)]
        [Display(Name = "Дом")]
        public string HouseNumber { get; set; }
        [StringLength(255)]
        [Display(Name = "Дополнение")]
        public string Additional { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models.ViewModel
{
    public class DriverViewModel
    {
        public int Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя - обязательно для ввода")]
        public string FirstName { get; set; }
        [StringLength(255)]
        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия - обязательно для ввода")]
        public string LastName { get; set; }
        [StringLength(255)]
        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Отчество - обязательно для ввода")]
        public string Patronymic { get; set; }
        [StringLength(255)]
        [Display(Name = "Позывной")]
        public string Call { get; set; }
        [StringLength(20)]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

    }
}
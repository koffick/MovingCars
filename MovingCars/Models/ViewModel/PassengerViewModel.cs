using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models.ViewModel
{
    public class PassengerViewModel
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
        public string Patronymic { get; set; }
        [StringLength(100)]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [StringLength(500)]
        [Display(Name = "Место работы")]
        public string Department { get; set; }
        [StringLength(255)]
        [Display(Name = "Должность")]
        public string Position { get; set; }
    }
}
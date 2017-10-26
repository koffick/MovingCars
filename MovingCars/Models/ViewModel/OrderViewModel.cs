using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models.ViewModel
{
    public class OrderViewModel
    {
        [Display(Name = "Начало поездки")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Завершение поездки")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Адрес подачи")]
        public string StartAddress { get; set; }

        [Display(Name = "Адрес назначения")]
        public string EndAddress { get; set; }

        [Display(Name = "За город")]
        public bool OutOfTown { get; set; }

        [Display(Name = "Пассажир")]
        public string Passenger { get; set; }

        [Display(Name = "Водитель")]
        public string Driver { get; set; }

        [Display(Name = "Комментарий")]
        public string Note { get; set; }
    }
}
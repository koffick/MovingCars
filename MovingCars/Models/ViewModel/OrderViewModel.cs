using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovingCars.Models.ViewModel
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Начало поездки")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Display(Name = "Завершение поездки")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }

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
        [Display(Name = "Идентификатор водителя")]
        public int DriverId { get; set; }

        [Display(Name = "Комментарий")]
        public string Note { get; set; }
    }
}
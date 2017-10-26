using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class PassengerController : BaseController<Passenger, PassengerViewModel, int>
    {
        public PassengerController()
            : base()
        {
            base.entities = base.db.Passengers;
        }

        protected override IQueryable<Passenger> Searching(string value, IQueryable<Passenger> query)
        {
            query = query.Where(p => p.FirstName.Contains(value) ||
                                    p.LastName.Contains(value) ||
                                    p.Patronymic.Contains(value)
                              );
            return query;
        }

        [HttpGet]
        public ActionResult LoadDataFromAD()
        {
            var users = GroupHelper.GetGroupFIOUsers("TMN\\Пользователи домена");
            foreach (var item in users)
            {
                if (base.entities.Where(w => w.Department == item.Department && w.FirstName == item.FirstName && w.LastName == item.LastName && w.Patronymic == item.Patronymic).FirstOrDefault() == null)
                {
                    base.entities.Add(item);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("List");
        }
    }
}
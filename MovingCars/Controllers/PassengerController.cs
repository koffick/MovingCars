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
    }
}
using AutoMapper;
using DataTables.Mvc;
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class DriverController : BaseController<Driver, DriverViewModel, int>
    {
        public DriverController()
            : base()
        {
            base.entities = base.db.Drivers;
        }

        protected override IQueryable<Driver> Searching(string value, IQueryable<Driver> query)
        {
            query = query.Where(p => p.FirstName.Contains(value) ||
                                    p.LastName.Contains(value) ||
                                    p.Patronymic.Contains(value)
                              );
            return query;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class DriverController : Controller
    {
        StorageContext db = new StorageContext();

        // GET: Driver
        public ActionResult List()
        {
            ViewBag.Message = "Список водителей, с возможностью редактирования.";
            var drivers = db.Drivers;
            return View(drivers.ToList());
        }
    }
}
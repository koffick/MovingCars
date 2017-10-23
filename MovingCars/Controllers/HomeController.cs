using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Описание приложения.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Адрес:";

            return View();
        }
    }
}
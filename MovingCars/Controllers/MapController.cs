using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;

namespace MovingCars.Controllers
{
    public class MapController : BaseMapController
    {

        public MapController()
            :base(new OrderMapProfile())
        {
        }

        // GET: Map
        public ActionResult Index()
        {
            var entyties = this.db.Drivers.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName + " " + s.Patronymic });
            ViewBag.CustomerID = new SelectList(entyties, "Id", "Name");
            return View();
        }

        public JsonResult GetTrack(int id)
       {
            var orderQuery = this.db.Orders.Where(w => w.Id == id).FirstOrDefault();
            var points = this.db.Points.Where(w => w.DriverId == orderQuery.DriverId && w.Date >= orderQuery.StartDate && w.Date <=
            (orderQuery.EndDate == null ? w.Date : (DateTime)orderQuery.EndDate)).OrderBy(o => o.Date).ToList();

            var pointList = new List<Double[]>();

            foreach (var item in points)
            {
                pointList.Add(new Double[] { item.Latitude, item.Longitude });
            }
            return Json(new { value = pointList.ToArray(), id = id, hint = orderQuery.StartDate.ToString("dd.MM.yy HH:mm") + (orderQuery.EndDate != null ? " - " + ((DateTime)orderQuery.EndDate).ToString("HH:mm") : "") + ", " + orderQuery.StartAddress + " - " + orderQuery.EndAddress }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrders(int id, int driverid)
        {
            var orderQuery = this.db.Orders.Where(w => w.DriverId == driverid);
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.AddDays(1).Date;
            if (id < 0)
            {
                startDate = startDate.AddDays(id);
            }
            if (id > 0)
            {
                startDate = startDate.AddDays(id);
                endDate = endDate.AddDays(id);
            }

            orderQuery = orderQuery.Where(w => w.StartDate >= startDate && w.StartDate < endDate);

            var orders = orderQuery.OrderBy(o => o.StartDate).ToList();
            var orderViews = base.mapper.Map<Order[], OrderViewModel[]>(orders.ToArray())
                .Select(s => new {
                    Id = s.Id,
                    text = s.StartDate.ToString("dd.MM.yy HH:mm") + (s.EndTime != null ? " - "+((DateTime)s.EndTime).ToString("HH:mm") : "") + ", " + s.StartAddress + " - " + s.EndAddress,
                    title = s.StartAddress + " - " + s.EndAddress
                });

            return Json(new { value = orderViews }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPoint(int id)
        {
            var lat = 57.15001 + (double)id / 100;
            var lon = 65.45002 + (double)id / 100;
            return Json(new { value = new Double[] { lat, lon } }, JsonRequestBehavior.AllowGet);
        }
    }
}
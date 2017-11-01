using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YandexAPI.Maps;
using System.IO;

namespace MovingCars.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index()
        {
            var db = new StorageContext();
            var entyties = db.Drivers.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName + " " + s.Patronymic });
            ViewBag.CustomerID = new SelectList(entyties, "Id", "Name");
            return View();
        }

        public ActionResult RenderImage()
        {
            YandexAPI.Maps.GeoCode geoCode = new GeoCode();

            string address = "Тюмень, " + "Харьковская 75/1";

            string ResultSearchObject = geoCode.SearchObject(address.Trim());
            ViewBag.Point = geoCode.GetPoint(ResultSearchObject);
            string ImageUrl = geoCode.GetUrlMapImage(ResultSearchObject, 16, 650, 450);

            return File(geoCode.DownloadMapByteArray(ImageUrl), "image/jpeg");
        }

        public JsonResult GetTrack(int id)
       {
            Double[][] arr = new Double[][]
                {
                    new Double[]{ 57.1443, 65.5943 },
                    new Double[]{ 57.1478, 65.5877 },
                    new Double[]{ 57.1459, 65.582 },
                    new Double[]{ 57.1561, 65.5617 },
                    new Double[]{ 57.1487, 65.5502 },
                    new Double[]{ 57.155, 65.5373 }
                };
            return Json(new { value = arr}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPoint(int id)
        {
            var lat = 57.15001 + (double)id / 100;
            var lon = 65.45002 + (double)id / 100;
            return Json(new { value = new Double[] { lat, lon } }, JsonRequestBehavior.AllowGet);
        }
    }
}
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class OrderController : BaseGenericController
    {
        public OrderController()
            :base(new OrderMapProfile())
        {

        }

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order
        public ActionResult Create()
        {
            var model = new OrderViewModel();
            model.StartDate = DateTime.Now;
            model.StartTime = DateTime.Now;
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Create(OrderViewModel newOrder)
        {
            Order order = mapper.Map<OrderViewModel, Order>(newOrder);

            this.db.Orders.Add(order);
            var task = db.SaveChangesAsync();
            await task;


            return RedirectToAction("Index", "Home");
        }

        public ActionResult AutocompleteSearch(string term)
        {
            var models = this.db.Passengers.Where(a => a.LastName.Contains(term))
                            .Select(a => new { value = a.LastName + " "  + a.FirstName + " " + a.Patronymic})
                            .Distinct();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

    }
}
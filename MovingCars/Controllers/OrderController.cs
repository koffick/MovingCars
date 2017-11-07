using DataTables.Mvc;
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using suggestionscsharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class OrderController : BaseGenericController<Order, OrderViewModel, int>
    {
        public OrderController()
            :base(new OrderMapProfile())
        {
            base.entities = base.db.Orders;
            ViewBag.Message = "";
            ViewBag.CloseUrl = "/Order/Close";
        }

        // GET: Order
        public override ActionResult Create()
        {
            var model = new OrderViewModel();
            model.StartDate = DateTime.Now;
            model.StartTime = DateTime.Now;
            return View(model);
        }


        [HttpPost]
        public override async Task<ActionResult> Create(OrderViewModel newOrder)
        {
            Order order = mapper.Map<OrderViewModel, Order>(newOrder);

            base.entities.Add(order);
            var task = db.SaveChangesAsync();
            await task;


            return RedirectToAction("List", "Order");
        }

        public override ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<Order> query = this.entities;
            var totalCount = query.Count();

            // Searching and sorting
            query = base.SearchEntities(requestModel, query);
            var filteredCount = query.Count();

            // Paging
            if (requestModel.Length != -1)
            {
                query = query.Skip(requestModel.Start).Take(requestModel.Length);
            }
            var data = query.Include(i => i.Driver).ToList().Select(s => 
            new
            {
                Id = s.Id,
                StartDate = s.StartDate.ToString("dd.MM.yyyy hh:mm"),
                EndDate = s.EndDate != null ? s.EndDate.Value.ToString("dd.MM.yyyy hh:mm") : s.EndDate.ToString(),
                StartAddress = s.StartAddress,
                EndAddress = s.EndAddress,
                Passenger = s.Passenger,
                Driver = s.Driver != null ? s.Driver.LastName + " " + s.Driver.FirstName + " " + s.Driver.Patronymic : "",
                Note = s.Note
            }).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);

        }

        public ActionResult PassengerAutocompleteSearch(string term)
        {
            var models = this.db.Passengers.Where(a => a.LastName.Contains(term))
                            .Select(a => new { value = a.LastName + " "  + a.FirstName + " " + a.Patronymic})
                            .Distinct().Take(10);

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DriverAutocompleteSearch(string term)
        {
            var models = this.db.Drivers.Where(a => a.LastName.Contains(term))
                            .Select(a => new { label = a.LastName + " " + a.FirstName + " " + a.Patronymic, value = a.Id })
                            .Distinct().Take(10);

            return Json(models, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddressAutocompleteSearch(string term)
        {
            var mainFilter = "Тюмень ";
            var token = "0426e29243b9b6e9445adecd9bf11cdc3f2b6997";
            var url = "https://suggestions.dadata.ru/suggestions/api/4_1/rs";
            var api = new SuggestClient(token, url);
            var addresses = api.QueryAddress(mainFilter + term);

            List<string> result = new List<string>();
            foreach (var item in addresses.suggestions)
            {
                var region = item.data.region_with_type;
                var str = item.value.Replace(region + ", ", "");
                result.Add(str);
            }

            return Json(result,
                        JsonRequestBehavior.AllowGet);
        }

        // GET: Asset/Edit/5
        public override ActionResult Edit(int id)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(Order), "x");
            var expr = Expression.Property(parameter, "Id");
            var val1 = Expression.Constant(id);
            Expression expression = Expression.Equal(expr, val1);
            var lambda = Expression.Lambda<Func<Order, bool>>(expression, new ParameterExpression[] { parameter });
            Order address = entities.Include(i => i.Driver).FirstOrDefault(lambda.Compile());


            OrderViewModel entityVM = mapper.Map<Order, OrderViewModel>(address);

            //if (Request.IsAjaxRequest())
            //    return PartialView("_EditPartial", entityVM);
            return View(entityVM);
        }

        // GET: Asset/Close/5
        public ActionResult Close(int id)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(Order), "x");
            var expr = Expression.Property(parameter, "Id");
            var val1 = Expression.Constant(id);
            Expression expression = Expression.Equal(expr, val1);
            var lambda = Expression.Lambda<Func<Order, bool>>(expression, new ParameterExpression[] { parameter });
            Order address = entities.FirstOrDefault(lambda.Compile());

            OrderViewModel entityVM = mapper.Map<Order, OrderViewModel>(address);
            if (entityVM.EndDate == null && entityVM.EndTime == null)
            {
                entityVM.EndDate = DateTime.Now;
                entityVM.EndTime = DateTime.Now;
            }
            //if (Request.IsAjaxRequest())
            //    return PartialView("_EditPartial", entityVM);
            return View("Edit", entityVM);
        }

        [HttpPost]
        public async Task<ActionResult> Close(OrderViewModel entityVM)
        {
            return await Edit(entityVM);
        }
        // POST: Asset/Edit/5
        [HttpPost]
        public override async Task<ActionResult> Edit(OrderViewModel entityVM)
        {
            Order entity = mapper.Map<OrderViewModel, Order>(entityVM);

            this.entities.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", entityVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("List");

        }
    }
}
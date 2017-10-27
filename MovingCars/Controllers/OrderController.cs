using DataTables.Mvc;
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
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
            var data = query.ToList().Select(s => 
            new
            {
                Id = s.Id,
                StartDate = s.StartDate.ToString("dd.MM.yyyy hh:mm"),
                EndDate = s.EndDate != null ? s.EndDate.Value.ToString("dd.MM.yyyy hh:mm") : s.EndDate.ToString(),
                StartAddress = s.StartAddress,
                EndAddress = s.EndAddress,
                Passenger = s.Passenger,
                Driver = s.Driver,
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
                            .Select(a => new { value = a.LastName + " " + a.FirstName + " " + a.Patronymic })
                            .Distinct().Take(10);

            return Json(models, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult AddressAutocompleteSearch(string term)
        {
            var strings = term.Trim().Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length == 1)
            {
                strings = term.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            IQueryable<Address> query = this.db.Addresses;

            if (strings.Length == 1)
            {
                query = SearchOne(query, strings[0]);
            }
            if (strings.Length == 2)
            {
                query = SearchTwo(query, strings);
            }
            if (strings.Length == 3)
            {
                query = SearchThree(query, strings);
            }

            var addresses = query.Select(a => new { value = a.District + ", " + a.City + ", " + a.Locality + ", " + a.Street + ", " + a.HouseNumber + ", " + a.Additional })
                        .Distinct().Take(10).ToList();
            List<string> result = new List<string>();
            foreach (var item in addresses)
            {
                var s = item.value.ToString().Replace(", ,", ",").Replace(", ,", ",").Replace(", ,", ",");
                if (s.Substring(0, 2) == ", ")
                {
                    s = s.Substring(2, s.Length - 2);
                }
                if (s.Substring(s.Length - 2, 2) == ", ")
                {
                    s = s.Substring(0, s.Length - 2);
                }
                result.Add(s);
            }

            return Json(result,
                        JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Address> SearchOne(IQueryable<Address> query, string term)
        {
            term = term.Trim();
            return query.Where(a => a.City.Contains(term) || a.Locality.Contains(term) || a.Street.Contains(term) || a.HouseNumber.Contains(term));
        }

        private IQueryable<Address> SearchTwo(IQueryable<Address> query, string[] terms)
        {
            var s1 = terms[0].Trim();
            var s2 = terms[1].Trim();

            int n;
            if (int.TryParse(s1, out n) || int.TryParse(s2, out n))
            {
                return query.Where(a => a.Street.Contains(s1) && (a.Street.Contains(s2) || (a.HouseNumber.Contains(s2))));
            }
            else
            {
                var q = query.Where(a =>
                    (a.City.Contains(s1) || a.Locality.Contains(s1) || a.Street.Contains(s1) || a.HouseNumber.Contains(s1))
                    &&
                    (a.City.Contains(s2) || a.Locality.Contains(s2) || a.Street.Contains(s2) || a.HouseNumber.Contains(s2))
                    );
                return q;
            }
        }

        private IQueryable<Address> SearchThree(IQueryable<Address> query, string[] terms)
        {
            var s1 = terms[0].Trim();
            var s2 = terms[1].Trim();
            var s3 = terms[2].Trim();

            int n;
            if (int.TryParse(s1, out n) && int.TryParse(s2, out n) && int.TryParse(s2, out n))
            {
                return query.Where(a => a.Street.Contains(s1) && (a.HouseNumber.Contains(s2)) && (a.Additional.Contains(s3)));
            }
            else
            {
                return query.Where(a =>
                    (a.City.Contains(s1) || a.Locality.Contains(s1) || a.Street.Contains(s1) || a.HouseNumber.Contains(s1) || a.Additional.Contains(s1))
                    &&
                    (a.City.Contains(s2) || a.Locality.Contains(s2) || a.Street.Contains(s2) || a.HouseNumber.Contains(s2) || a.Additional.Contains(s2))
                    &&
                    (a.City.Contains(s3) || a.Locality.Contains(s3) || a.Street.Contains(s3) || a.HouseNumber.Contains(s3) || a.Additional.Contains(s3))
                    );
            }
        }

        // GET: Asset/Edit/5
        public override ActionResult Edit(int id)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(Order), "x");
            var expr = Expression.Property(parameter, "Id");
            var val1 = Expression.Constant(id);
            Expression expression = Expression.Equal(expr, val1);
            var lambda = Expression.Lambda<Func<Order, bool>>(expression, new ParameterExpression[] { parameter });
            Order address = entities.FirstOrDefault(lambda.Compile());

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
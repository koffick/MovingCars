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
    public class DriverController : Controller
    {
        private StorageContext db;
        private MapperConfiguration config;
        private IMapper mapper;

        public DriverController()
        {
            this.db = new StorageContext();
            this.config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DriverMapProfile>();
            });
            this.mapper = this.config.CreateMapper();
        }

        // GET: Driver
        public ActionResult List()
        {
            ViewBag.Message = "Список водителей, с возможностью редактирования.";
            var drivers = db.Drivers;
            return View(drivers.ToList());
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<Driver> query = db.Drivers;
            var totalCount = query.Count();

            // Searching and sorting
            query = SearchAssets(requestModel, query);
            var filteredCount = query.Count();

            // Paging
            if (requestModel.Length != -1)
            {
                query = query.Skip(requestModel.Start).Take(requestModel.Length);
            }
            var data = query.Select(s => s).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);

        }


        // GET: Asset/Create
        public ActionResult Create()
        {
            var model = new DriverViewModel();
            return View("_CreatePartial", model);
        }

        //POST: Asset/Create
        [HttpPost]
        public async Task<ActionResult> Create(DriverViewModel driverVM)
        {


            if (!ModelState.IsValid)
                return View("_CreatePartial", driverVM);

            Driver driver = mapper.Map<DriverViewModel, Driver>(driverVM);

            db.Drivers.Add(driver);
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Не удалось добавить водителя");
                return View("_CreatePartial", driverVM);
            }

            return Content("success");
        }

        // GET: Asset/Edit/5
        public ActionResult Edit(int id)
        {
            var driver = db.Drivers.FirstOrDefault(x => x.Id == id);

            DriverViewModel driverViewModel = mapper.Map<Driver, DriverViewModel>(driver);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", driverViewModel);
            return View(driverViewModel);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(DriverViewModel driverVM)
        {

            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", driverVM);
            }

            Driver driver = mapper.Map<DriverViewModel, Driver>(driverVM);

            db.Drivers.Attach(driver);
            db.Entry(driver).State = EntityState.Modified;
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", driverVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("List");

        }

        private IQueryable<Driver> SearchAssets(IDataTablesRequest requestModel, IQueryable<Driver> query)
        {

            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.FirstName.Contains(value) ||
                                         p.LastName.Contains(value) ||
                                         p.Patronymic.Contains(value)
                                   );
            }

            var filteredCount = query.Count();

            // Sort
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            query = query.OrderBy(orderByString == string.Empty ? "Id asc" : orderByString);

            return query;

        }
    }
}
using AutoMapper;
using DataTables.Mvc;
using MovingCars.Mapping;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class AddressController : Controller
    {
        private StorageContext db;
        private IMapper mapper;

        public AddressController()
        {
            this.db = new StorageContext();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityMapProfile<AddressViewModel, Address>>();
            });
            this.mapper = config.CreateMapper();
        }

        // GET: Фввкуы
        public ActionResult List()
        {
            ViewBag.Message = "Справочник адресов, с возможностью редактирования.";
            var addresses = db.Addresses;
            return View(addresses.ToList());
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<Address> query = db.Addresses;
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
            var model = new AddressViewModel();
            return View("_CreatePartial", model);
        }

        //POST: Asset/Create
        [HttpPost]
        public async Task<ActionResult> Create(AddressViewModel addressVM)
        {


            if (!ModelState.IsValid)
                return View("_CreatePartial", addressVM);

            Address driver = mapper.Map<AddressViewModel, Address>(addressVM);

            db.Addresses.Add(driver);
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Не удалось добавить водителя");
                return View("_CreatePartial", addressVM);
            }

            return Content("success");
        }

        // GET: Asset/Edit/5
        public ActionResult Edit(string id)
        {
            var address = db.Addresses.FirstOrDefault(x => x.Id == id);

            AddressViewModel addressViewModel = mapper.Map<Address, AddressViewModel>(address);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", addressViewModel);
            return View(addressViewModel);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(AddressViewModel addressVM)
        {

            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", addressVM);
            }

            Address address = mapper.Map<AddressViewModel, Address>(addressVM);

            db.Addresses.Attach(address);
            db.Entry(address).State = EntityState.Modified;
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", addressVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("List");

        }

        private IQueryable<Address> SearchAssets(IDataTablesRequest requestModel, IQueryable<Address> query)
        {

            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.HouseNumber.Contains(value) ||
                                         p.Street.Contains(value) ||
                                         p.Additional.Contains(value) ||
                                         p.District.Contains(value) ||
                                         p.City.Contains(value) ||
                                         p.Region.Contains(value)
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
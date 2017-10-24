using AutoMapper;
using DataTables.Mvc;
using MovingCars.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace MovingCars.Controllers
{
    public class BaseController<TEntity, TView, TTypeId> : Controller where TEntity: class
    {
        protected StorageContext db;
        protected IMapper mapper;
        protected string titleMessage = "";
        protected DbSet<TEntity> entities;


        public BaseController()
        {
            this.db = new StorageContext();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityMapProfile<TView, TEntity>>();
            });
            this.mapper = config.CreateMapper();
        }

        public ActionResult List()
        {
            ViewBag.Message = "Справочник адресов, с возможностью редактирования.";
            return View(entities.ToList());
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<TEntity> query = this.entities;
            var totalCount = query.Count();

            // Searching and sorting
            query = SearchEntities(requestModel, query);
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
            var model = Activator.CreateInstance(typeof(TView));
            return View("_CreatePartial", model);
        }

        //POST: Asset/Create
        [HttpPost]
        public async Task<ActionResult> Create(TView entityVM)
        {
            if (!ModelState.IsValid)
                return View("_CreatePartial", entityVM);

            TEntity entity = mapper.Map<TView, TEntity>(entityVM);

            this.entities.Add(entity);
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Не удалось добавить водителя");
                return View("_CreatePartial", entityVM);
            }

            return Content("success");
        }

        // GET: Asset/Edit/5
        public ActionResult Edit(TTypeId id)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            var expr = Expression.Property(parameter, "Id");
            var val1 = Expression.Constant(id);
            Expression expression = Expression.Equal(expr, val1);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, new ParameterExpression[] { parameter });
            TEntity address = entities.FirstOrDefault(lambda.Compile());

            TView entityVM = mapper.Map<TEntity, TView>(address);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", entityVM);
            return View(entityVM);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(TView entityVM)
        {

            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", entityVM);
            }

            TEntity entity = mapper.Map<TView, TEntity>(entityVM);

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

        private IQueryable<TEntity> SearchEntities(IDataTablesRequest requestModel, IQueryable<TEntity> query)
        {

            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = Searching(value, query);
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

        virtual protected IQueryable<TEntity> Searching(string value, IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
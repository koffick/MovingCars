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
     public class AddressController : BaseController<Address, AddressViewModel, string>
    {
        public AddressController()
            : base()
        {
            base.entities = base.db.Addresses;

        }

         protected override IQueryable<Address> Searching(string value, IQueryable<Address> query)
        {
            query = query.Where(p => p.HouseNumber.Contains(value) ||
                                    p.Street.Contains(value) ||
                                    p.Additional.Contains(value) ||
                                    p.District.Contains(value) ||
                                    p.City.Contains(value) ||
                                    p.Region.Contains(value)
                              );
            return query;
        }
    }
}
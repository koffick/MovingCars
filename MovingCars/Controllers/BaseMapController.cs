﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public abstract class BaseMapController : Controller
    {
        protected StorageContext db;
        protected IMapper mapper;


        public BaseMapController(Profile profile)
        {
            this.db = new StorageContext();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profile);
            });
            this.mapper = config.CreateMapper();
        }
    }
}
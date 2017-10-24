using AutoMapper;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovingCars.Mapping
{
    public class DriverMapProfile : Profile
    {
        public DriverMapProfile()
        {
            CreateMap<DriverViewModel, Driver>();
        }
    }
}
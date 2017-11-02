using AutoMapper;
using MovingCars.Models;
using MovingCars.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovingCars.Mapping
{
    public class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<OrderViewModel, Order>()
                .ForMember(m => m.StartDate, opt => opt.MapFrom(src => new DateTime(src.StartDate.Year, src.StartDate.Month, src.StartDate.Day, src.StartTime.Hour, src.StartTime.Minute, src.StartTime.Second)))
                .ForMember(m => m.EndDate, opt => opt.MapFrom(src => (src.EndDate == null || src.EndTime == null  ?  (DateTime?)null :
                new DateTime(src.EndDate.GetValueOrDefault().Year, src.EndDate.GetValueOrDefault().Month, src.EndDate.GetValueOrDefault().Day, src.EndTime.GetValueOrDefault().Hour, src.EndTime.GetValueOrDefault().Minute, src.EndTime.GetValueOrDefault().Second))))
                .ForMember(x => x.Driver, opt => opt.Ignore());
            CreateMap<Order, OrderViewModel>()
                .ForMember(m => m.StartTime, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(m => m.EndTime, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(m => m.Driver, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.LastName + " " + src.Driver.FirstName + " " + src.Driver.Patronymic : ""));
        }
    }
}
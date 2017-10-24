using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovingCars.Mapping
{
    public class EntityMapProfile<TView, TEntity> : Profile
    {
        public EntityMapProfile()
        {
            CreateMap<TView, TEntity>();
            CreateMap<TEntity, TView>();
        }
    }
}
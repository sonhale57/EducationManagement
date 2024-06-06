using AutoMapper;
using Hangfire;
using Newtonsoft.Json;
using SuperbrainManagement.DTOs;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SuperbrainManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            // Register the IMapper instance
            AutoMapperConfig.Mapper = mapper;
        }

        public static class AutoMapperConfig
        {
            public static IMapper Mapper { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ScheduleViewDTO, Schedule>()
                    .ForMember(dest => dest.IdWeek, opt => opt.MapFrom(src => int.Parse(src.IdWeek)))                    
                    .ForMember(dest => dest.FromHour, opt => opt.MapFrom(src => DateTime.Parse(src.FromHour)))
                    .ForMember(dest => dest.ToHour, opt => opt.MapFrom(src => DateTime.Parse(src.ToHour)))
                    .ForMember(dest => dest.IdEmployee, opt => opt.MapFrom(src => src.EmployeeId))
                    .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                    .ForMember(dest => dest.IdRoom, opt => opt.MapFrom(src => src.RoomId))
                    .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.IdUser))
                    .ForMember(dest => dest.IdClass, opt => opt.MapFrom(src => src.IdClass));

            }
        }
    }
}

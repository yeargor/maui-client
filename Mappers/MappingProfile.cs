using AutoMapper;
using MauiDemo2.Dtos;
using MauiDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiDemo2.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RouteMainPage, TripResponseDto>().ReverseMap();
        }
    }
}

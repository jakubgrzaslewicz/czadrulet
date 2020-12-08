using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CzadRuletAPI.Entities;
using CzadRuletAPI.Models;

namespace CzadRuletAPI.Mappers
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<AuthenticateModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}
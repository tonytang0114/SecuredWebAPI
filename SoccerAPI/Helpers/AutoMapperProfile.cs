using AutoMapper;
using SoccerAPI.Entities;
using SoccerAPI.Models;
using SoccerAPI.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<ConfirmNewPassword, User>();
            CreateMap<RecoveryPasswordEmail, RecoveryToken>();
            CreateMap<RecoveryPasswordSMS, RecoveryToken>();
        }
    }
}

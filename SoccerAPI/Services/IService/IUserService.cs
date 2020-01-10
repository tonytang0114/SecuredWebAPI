using SoccerAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Services.IService
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        User Create(User user, string password);

        User GetById(int id);

        //void ForgetPassword(User user);
        void Update(User user, string password = null);
    }
}

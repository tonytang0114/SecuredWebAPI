using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Models.User
{
    public class ConfirmNewPassword
    {
        [Required]
        public string Username;

        [Required]
        public string Password;
    }
}

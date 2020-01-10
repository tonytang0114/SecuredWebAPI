using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Models.User
{
    public class RecoveryPasswordEmail
    {
        [Required]
        public string Password_Reminder_Token { get; set; }

        [Required]
        public DateTime Password_Reminder_Expire { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        public string UserId { get; set; }
    }
}

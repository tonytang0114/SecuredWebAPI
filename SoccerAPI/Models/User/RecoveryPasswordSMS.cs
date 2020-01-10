using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Models.User
{
    public class RecoveryPasswordSMS
    {
        [Required]
        public string SMS_Code { get; set; }

        [Required]
        public DateTime SMS_Code_Expire { get; set; } = DateTime.Now.AddMinutes(5);

        [Required]
        public string UserId { get; set; }
    }
}

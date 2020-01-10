using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Entities
{
    public class RecoveryToken
    {
        public int Id { get; set; }

        public string Password_Reminder_Token { get; set; }

        public string SMS_Code { get; set; }
        
        public DateTime SMS_Code_Expire { get; set; }

        public DateTime Password_Reminder_Expire { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}

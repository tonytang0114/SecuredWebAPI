using SoccerAPI.Entities;
using SoccerAPI.Helpers;
using SoccerAPI.Models.Enum;
using SoccerAPI.Models.User;
using SoccerAPI.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerAPI.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;


        private readonly IAwsEmailService _awsEmailService;

        public UserService(DataContext context, IAwsEmailService awsEmailService)
        {
            _context = context;
            _awsEmailService = awsEmailService;
        }

        public User Authenticate(string UserName, string password)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.UserName == UserName);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new Exception("UserName \"" + user.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void GenerateVericationToken(RecoveryToken recoveryToken, RecoveryPasswordWay method)
        {
            // If user exists, update the target row with the email token
            // else insert a new row
            if(recoveryToken.UserId != 0)
            {
                if(method == RecoveryPasswordWay.Email)
                {
                    recoveryToken.Password_Reminder_Token = GeneralHelper.GenerateGuid();
                    _context.RecoveryToken.Update(recoveryToken);
                }
                if(method == RecoveryPasswordWay.SMS)
                {
                    recoveryToken.SMS_Code = GeneralHelper.GenerateOTPCode();
                    _context.RecoveryToken.Update(recoveryToken);
                }
            }
            else
            {
                if (method == RecoveryPasswordWay.Email)
                {
                    recoveryToken.Password_Reminder_Token = GeneralHelper.GenerateGuid();
                    _context.RecoveryToken.Add(recoveryToken);
                }
                if (method == RecoveryPasswordWay.SMS)
                {
                    recoveryToken.SMS_Code = GeneralHelper.GenerateOTPCode();
                    _context.RecoveryToken.Add(recoveryToken);
                }
            }
        }

        public void UpdatePassword(User userParam)
        {
            var user = _context.Users.Find(userParam.UserName);

            if (user == null)
                throw new Exception("User not found");

            _context.Users.Update(userParam);
        }

        public void Update(User userParam, string password=null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrWhiteSpace(userParam.UserName) && userParam.UserName != user.UserName)
            {
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new Exception("UserName " + userParam.UserName + " is already taken");

                user.UserName = userParam.UserName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;


            if (!string.IsNullOrWhiteSpace(userParam.Email))
                user.Email = userParam.Email;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();

        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.");

            if (storedHash.Length != 64) 
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) 
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Helpers
{
    public static class PasswordHasher
    {
        public static (string passwordHash, string salt) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return (passwordHash, salt);
        }
        //public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        //{
        //    var saltBytes = Convert.FromBase64String(storedSalt);
        //    using var hmac = new HMACSHA512(saltBytes);
        //    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    var storedHashBytes = Convert.FromBase64String(storedHash);
        //    return computedHash.SequenceEqual(storedHashBytes);
        //}
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PersonRegistrationSystem.BusinessLogic.Helpers
{
    public static class PasswordVerifier
    {
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var storedHashBytes = Convert.FromBase64String(storedHash);
            return computedHash.SequenceEqual(storedHashBytes);
        }
    }
}

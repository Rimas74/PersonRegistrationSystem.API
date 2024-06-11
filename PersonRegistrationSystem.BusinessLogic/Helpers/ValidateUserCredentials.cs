using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;
using System;

namespace PersonRegistrationSystem.BusinessLogic.Helpers
{
    public static class ValidateUserCredentials
    {
        public static void Validate(User user, string password, ILogger logger)
        {
            if (user == null)
            {
                logger.LogWarning("Invalid username.");
                throw new UnauthorizedAccessException("Invalid username.");
            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordValid)
            {
                logger.LogWarning($"Invalid password for username {user.Username}.");
                throw new UnauthorizedAccessException("Invalid password.");
            }
        }
    }
}

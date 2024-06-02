using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Repositories
{


    public class UserRepository : IUserRepository
    {
        private readonly PersonRegistrationContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(PersonRegistrationContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(User user)
        {
            _logger.LogInformation($"Adding new user with username: {user.Username}");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User {user.Username} added successfully.");
        }

        public async Task DeleteAsync(int userId)
        {
            _logger.LogInformation($"Deleting user with ID: {userId}");
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                _logger.LogInformation($"User with ID: {userId} deleted successfully");
            }
            else
            {
                _logger.LogWarning($"User with ID: {userId} not found");
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            _logger.LogInformation("Getting all users from database.");
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            _logger.LogInformation($"From database getting user by ID: {userId} from database.");
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            _logger.LogInformation($"From database getting user by username: {username}.");
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateAsync(User user)
        {
            _logger.LogInformation($"Updating user with ID: {user.Id}");
            var userToUpdate = await GetByIdAsync(user.Id);
            if (userToUpdate != null)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                _logger.LogInformation($"User with ID: {user.Id} updated successfully");
            }
            else
            {
                _logger.LogWarning($"User with ID: {user.Id} not found for update");
            }
        }
    }
}

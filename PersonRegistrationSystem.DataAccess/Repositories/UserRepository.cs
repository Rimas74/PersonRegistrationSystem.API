using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System.Collections.Generic;
using System.IO;
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

        public async Task<User> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Persons)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID: {userId} has been removed from the database.");
            return user;
        }


        public async Task<IEnumerable<User>> GetAllAsync()
        {
            _logger.LogInformation("Getting all users from database.");
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            _logger.LogInformation($"From database getting user by ID: {userId} from database.");
            return await _context.Users
                .Include(u => u.Persons)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            _logger.LogInformation($"From database getting user by username: {username}.");
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        }
}

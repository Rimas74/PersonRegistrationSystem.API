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
                .ThenInclude(p => p.PlaceOfResidence)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            foreach (var person in user.Persons)
            {
                _logger.LogInformation($"Deleting person with ID: {person.Id} for user ID: {userId}");

                if (!string.IsNullOrEmpty(person.ProfilePhotoPath) && File.Exists(person.ProfilePhotoPath))
                {
                    File.Delete(person.ProfilePhotoPath);
                }

                var placeOfResidence = await _context.PlacesOfResidence.FirstOrDefaultAsync(p => p.PersonId == person.Id);
                if (placeOfResidence != null)
                {
                    _context.PlacesOfResidence.Remove(placeOfResidence);
                }

                _context.Persons.Remove(person);
                _logger.LogInformation($"Person with ID: {person.Id} has been removed from the database.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

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

        public async Task UpdateAsync(User user)
        {
            _logger.LogInformation($"Updating user with ID: {user.Id}");
            var userToUpdate = await GetByIdAsync(user.Id);
            if (userToUpdate != null)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User with ID: {user.Id} updated successfully");
            }
            else
            {
                _logger.LogWarning($"User with ID: {user.Id} not found for update");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
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

        public UserRepository(PersonRegistrationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FindAsync(username);
        }

        public async Task UpdateAsync(User user)
        {
            var userToUpdate = await GetByIdAsync(user.Id);
            if (userToUpdate != null)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }
    }
}

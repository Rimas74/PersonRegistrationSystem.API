using PersonRegistrationSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<User> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);
    }
}

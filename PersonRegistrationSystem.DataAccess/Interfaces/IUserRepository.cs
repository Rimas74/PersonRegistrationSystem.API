using PersonRegistrationSystem.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<User> GetByIdAsync(int userId);
        //Task UpdateAsync(User user);
        Task<User> DeleteUserAsync(int userId);
    }
}

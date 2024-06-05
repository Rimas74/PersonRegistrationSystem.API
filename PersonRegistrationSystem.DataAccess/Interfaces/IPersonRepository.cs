using PersonRegistrationSystem.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllPersonsByUserIdAsync(int userId);
        Task<Person> GetByIdAsync(int id);
        Task CreateAsync(Person person);
        Task CreatePlaceOfResidenceAsync(PlaceOfResidence placeOfResidence);
        Task UpdateAsync(Person person);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string personalCode);
    }
}

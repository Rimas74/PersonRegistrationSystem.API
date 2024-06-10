using PersonRegistrationSystem.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllPersonsByUserIdAsync(int userId);
        Task<Person> GetByIdAsync(int id);
        Task<bool> PersonalCodeExistsForUserAsync(int userId, string personalCode);
        Task CreateAsync(Person person);
        Task<bool> EmailExistsAsync(string email);

        Task CreatePlaceOfResidenceAsync(PlaceOfResidence placeOfResidence);
        Task UpdatePersonDetailsAsync(Person person);
        Task UpdatePlaceOfResidenceAsync(PlaceOfResidence placeOfResidence);
        Task DeleteAsync(int id);
    }
}

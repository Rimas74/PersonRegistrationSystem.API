using PersonRegistrationSystem.Common.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Interfaces
{
    public interface IPersonService
    {
        Task<PersonDTO> CreatePersonAsync(int userId, PersonCreateDTO personCreateDTO);
        Task<PersonDTO> DeletePersonAsync(int userId, int personId);
        Task<IEnumerable<PersonDTO>> GetAllPersonsByUserIdAsync(int userId);
        Task<PersonDTO> GetPersonByIdAsync(int userId, int personId);
        Task<PersonGetImageDTO> GetPersonImageAsync(int userId, int personId);
        Task<PersonDTO> UpdatePersonDetailsAsync(int userId, int personId, PersonUpdateDetailsDTO personUpdateDetailsDTO);
        Task<PersonDTO> UpdatePersonImageAsync(int userId, int personId, PersonUpdateImageDTO personUpdateImageDTO);
    }
}

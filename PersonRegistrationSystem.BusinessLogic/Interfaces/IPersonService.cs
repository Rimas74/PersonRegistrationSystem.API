using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Interfaces
{
    public interface IPersonService
    {
        Task<PersonDTO> CreatePersonAsync(int userId, PersonCreateDTO personCreateDTO);
        Task<PersonDTO> UpdatePersonAsync(int personId, PersonUpdateDTO personUpdateDTO);
        Task<PersonDTO> DeletePersonAsync(int userId, int personId);

        Task<IEnumerable<PersonDTO>> GetAllPersonsByUserIdAsync(int userId);
        Task<PersonDTO> GetPersonByIdAsync(int userId, int personId);
    }
}

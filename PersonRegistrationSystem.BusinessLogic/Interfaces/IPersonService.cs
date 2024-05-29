using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Interfaces
{
    public interface IPersonService
    {
        Task<PersonDTO> CreatePersonAsync(PersonCreateDTO personCreateDTO);
        Task<PersonDTO> UpdatePersonAsync(int personId, PersonUpdateDTO personUpdateDTO);
        Task<bool> DeletePersonAsync(int personId);

        Task<IEnumerable<PersonDTO>> GetAllPersonsAsync();
        Task<PersonDTO> GetPersonById(int personId);
    }
}

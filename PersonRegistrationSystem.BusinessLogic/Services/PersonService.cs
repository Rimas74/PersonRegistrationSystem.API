using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class PersonService : IPersonService
    {
        public Task<PersonDTO> CreatePersonAsync(PersonCreateDTO personCreateDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePersonAsync(int personId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonDTO>> GetAllPersonsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PersonDTO> GetPersonById(int personId)
        {
            throw new NotImplementedException();
        }

        public Task<PersonDTO> UpdatePersonAsync(int personId, PersonUpdateDTO personUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}

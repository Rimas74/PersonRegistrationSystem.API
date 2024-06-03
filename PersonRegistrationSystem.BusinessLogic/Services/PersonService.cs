using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository personRepository, IMapper mapper, ILogger<PersonService> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PersonDTO> CreatePersonAsync(int userId, PersonCreateDTO personCreateDTO)
        {
            _logger.LogInformation($"Creating person for user ID: {userId}");

            if (!PersonalCodeValidator.Validate(personCreateDTO.PersonalCode))
            {
                throw new ArgumentException("Invalid Personal Identification Code.");
            }

            var person = _mapper.Map<Person>(personCreateDTO);
            person.UserId = userId;

            if (personCreateDTO.ProfilePhoto != null)
            {
                var filePath = Path.Combine("PersonPhoto", $"{DateTime.Now:yyyyMMddHHmmss}.jpg");
                ImageHelper.SaveResizedImage(filePath, personCreateDTO.ProfilePhoto, 200, 200);
                person.ProfilePhotoPath = filePath;
            }

            await _personRepository.CreateAsync(person);

            _logger.LogInformation($"Person created for user ID: {userId}");
            return _mapper.Map<PersonDTO>(person);
        }


        public Task<bool> DeletePersonAsync(int personId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PersonDTO>> GetAllPersonsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Retrieving all persons for user ID: {UserId}", userId);
            var persons = await _personRepository.GetAllPersonsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<PersonDTO>>(persons);
        }

        public async Task<PersonDTO> GetPersonByIdAsync(int userId, int personId)
        {
            _logger.LogInformation($"Retrieving person with ID: {personId} for user ID: {userId}");
            var person = await _personRepository.GetByIdAsync(personId);
            if (person == null || person.UserId != userId)
            {
                _logger.LogWarning($"Person with ID: {personId} not found or access denied for user ID: {userId}");
                throw new KeyNotFoundException("Person not found.");
            }
            return _mapper.Map<PersonDTO>(person);
        }

        public Task<PersonDTO> UpdatePersonAsync(int personId, PersonUpdateDTO personUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}

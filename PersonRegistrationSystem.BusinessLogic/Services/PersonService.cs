using AutoMapper;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

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

        var existingPerson = await _personRepository.PersonalCodeExistsForUserAsync(userId, personCreateDTO.PersonalCode);
        if (existingPerson)
        {
            throw new ArgumentException("A person with this Personal Identification Code already exists.");
        }

        var person = _mapper.Map<Person>(personCreateDTO);
        person.UserId = userId;

        if (personCreateDTO.ProfilePhoto != null)
        {
            var directory = "PersonPhoto";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = $"{person.Name}_{person.LastName}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            var filePath = Path.Combine(directory, filename);
            ImageHelper.SaveResizedImage(filePath, personCreateDTO.ProfilePhoto, 200, 200);
            person.ProfilePhotoPath = filePath;
        }


        await _personRepository.CreateAsync(person);

        var placeOfResidence = _mapper.Map<PlaceOfResidence>(personCreateDTO.PlaceOfResidence);
        placeOfResidence.PersonId = person.Id;


        await _personRepository.CreatePlaceOfResidenceAsync(placeOfResidence);

        _logger.LogInformation($"Person created for user ID: {userId}");
        return _mapper.Map<PersonDTO>(person);
    }

    public async Task<PersonDTO> DeletePersonAsync(int userId, int personId)
    {
        _logger.LogInformation($"Deleting person with ID: {personId} for user ID: {userId}");

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null || person.UserId != userId)
        {
            _logger.LogWarning($"Person with ID: {personId} not found or access denied for user ID: {userId}");
            throw new KeyNotFoundException("Person not found.");
        }

        await _personRepository.DeleteAsync(personId);
        _logger.LogInformation($"Person with ID: {personId} deleted successfully for user ID: {userId}");
        return _mapper.Map<PersonDTO>(person);
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

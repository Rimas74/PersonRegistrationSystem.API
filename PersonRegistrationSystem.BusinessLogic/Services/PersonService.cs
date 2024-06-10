using AutoMapper;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Enums;
using PersonRegistrationSystem.DataAccess.Interfaces;

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

        var existingPerson = await _personRepository.PersonalCodeExistsForUserAsync(userId, personCreateDTO.PersonalCode);
        if (existingPerson)
        {
            throw new ArgumentException("A person with this Personal Identification Code already exists.");
        }

        var emailExists = await _personRepository.EmailExistsAsync(personCreateDTO.Email);
        if (emailExists)
        {
            throw new ArgumentException("A person with this email already exists.");
        }
        if (!Enum.TryParse(personCreateDTO.Gender, out Gender gender))
        {
            throw new ArgumentException("Invalid gender value.");
        }

        var person = _mapper.Map<Person>(personCreateDTO);
        person.UserId = userId;
        person.Gender = gender;

        if (personCreateDTO.ProfilePhoto != null)
        {
            var filename = ImageHelper.GenerateImageFileName(person.Name, person.LastName);
            var filePath = ImageHelper.GetImageFilePath(filename);
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

        ImageHelper.DeleteImage(person.ProfilePhotoPath);

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
        if (person == null)
        {
            _logger.LogWarning($"Person with ID: {personId} not found for user ID: {userId}");
            throw new KeyNotFoundException("Person not found.");
        }
        if (person.UserId != userId)
        {
            _logger.LogWarning($"User ID: {userId} is not authorized to access person ID: {personId}");
            throw new UnauthorizedAccessException("Access is denied.");
        }
        return _mapper.Map<PersonDTO>(person);
    }

    public async Task<PersonGetImageDTO> GetPersonImageAsync(int userId, int personId)
    {
        _logger.LogInformation($"Retrieving picture for person ID: {personId} for user ID: {userId}");

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning($"Person with ID: {personId} not found for user ID: {userId}");
            throw new KeyNotFoundException("Person not found.");
        }
        if (person.UserId != userId)
        {
            _logger.LogWarning($"Access denied for user ID: {userId} to person ID: {personId}");
            throw new UnauthorizedAccessException("Access denied.");
        }

        var personImageDTO = new PersonGetImageDTO
        {
            ProfilePhoto = await File.ReadAllBytesAsync(person.ProfilePhotoPath),
            ProfilePhotoPath = person.ProfilePhotoPath
        };

        _logger.LogInformation($"Picture for person ID: {personId} retrieved successfully for user ID: {userId}");

        return personImageDTO;
    }

    public async Task<PersonDTO> UpdatePersonDetailsAsync(int userId, int personId, PersonUpdateDetailsDTO personUpdateDetailsDTO)
    {
        _logger.LogInformation($"Updating person details for person ID: {personId} by user ID: {userId}");

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning($"Person with ID: {personId} not found");
            throw new KeyNotFoundException("Person not found.");
        }
        if (person.UserId != userId)
        {
            _logger.LogWarning($"User ID: {userId} is not authorized to access person ID: {personId}");
            throw new UnauthorizedAccessException("Access is denied.");
        }

        _mapper.Map(personUpdateDetailsDTO, person);

        if (Enum.TryParse(personUpdateDetailsDTO.Gender, out Gender gender))
        {
            person.Gender = gender;
        }
        else
        {
            _logger.LogWarning($"Invalid gender value: {personUpdateDetailsDTO.Gender} for person ID: {personId}");
            throw new ArgumentException("Invalid gender value.");
        }

        await _personRepository.UpdatePersonDetailsAsync(person);
        await _personRepository.UpdatePlaceOfResidenceAsync(person.PlaceOfResidence);

        _logger.LogInformation($"Person details updated for person ID: {personId} by user ID: {userId}");
        return _mapper.Map<PersonDTO>(person);
    }

    public async Task<PersonDTO> UpdatePersonImageAsync(int userId, int personId, PersonUpdateImageDTO personUpdateImageDTO)
    {
        _logger.LogInformation($"Updating person image for person ID: {personId} by user ID: {userId}");

        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning($"Person with ID: {personId} not found");
            throw new KeyNotFoundException("Person not found.");
        }
        if (person.UserId != userId)
        {
            _logger.LogWarning($"User ID: {userId} is not authorized to access person ID: {personId}");
            throw new UnauthorizedAccessException("Access is denied.");
        }

        ImageHelper.DeleteImage(person.ProfilePhotoPath);

        var filename = ImageHelper.GenerateImageFileName(person.Name, person.LastName);
        var filePath = ImageHelper.GetImageFilePath(filename);
        ImageHelper.SaveResizedImage(filePath, personUpdateImageDTO.ProfilePhoto, 200, 200);
        person.ProfilePhotoPath = filePath;

        await _personRepository.UpdatePersonDetailsAsync(person);

        _logger.LogInformation($"Person image updated for person ID: {personId} by user ID: {userId}");
        return _mapper.Map<PersonDTO>(person);
    }

}

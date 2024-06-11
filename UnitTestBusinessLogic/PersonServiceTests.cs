using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.BusinessLogic.Services;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Enums;
using PersonRegistrationSystem.DataAccess.Helpers;
using PersonRegistrationSystem.DataAccess.Interfaces;
using UnitTestHelpers.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTestBusinessLogic
{
    public class PersonServiceTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private PersonRegistrationContext _context;
        private PersonService _personService;
        private Mock<IPersonRepository> _mockPersonRepository;
        private IMapper _mapper;
        private Mock<ILogger<PersonService>> _mockLogger;

        public PersonServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
            _mockLogger = new Mock<ILogger<PersonService>>();
            _mockPersonRepository = new Mock<IPersonRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonDTO>().ReverseMap();
                cfg.CreateMap<PersonCreateDTO, Person>().ReverseMap();
                cfg.CreateMap<PersonUpdateDetailsDTO, Person>().ReverseMap();
                cfg.CreateMap<PersonUpdateImageDTO, Person>().ReverseMap();
                cfg.CreateMap<PlaceOfResidenceCreateDTO, PlaceOfResidence>().ReverseMap();
                cfg.CreateMap<PlaceOfResidenceUpdateDTO, PlaceOfResidence>().ReverseMap();
                cfg.CreateMap<PlaceOfResidence, PlaceOfResidenceDTO>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _personService = new PersonService(_mockPersonRepository.Object, _mapper, _mockLogger.Object);

            await SeedData();
        }

        private async Task<PersonRegistrationContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<PersonRegistrationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new PersonRegistrationContext(options);
            await context.Database.EnsureCreatedAsync();

            context.Users.RemoveRange(context.Users);
            context.Persons.RemoveRange(context.Persons);
            context.PlacesOfResidence.RemoveRange(context.PlacesOfResidence);
            await context.SaveChangesAsync();

            return context;
        }

        private async Task SeedData()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "user1",
                    PasswordHash = "hashedpassword1",
                    Salt = "salt1",
                    Role = "User"
                },
                new User
                {
                    Id = 2,
                    Username = "user2",
                    PasswordHash = "hashedpassword2",
                    Salt = "salt2",
                    Role = "Admin"
                }
            };

            var persons = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = "Person1",
                    LastName = "LastName1",
                    Gender = Gender.Male,
                    Birthday = new DateTime(1990, 1, 1),
                    PersonalCode = "12345678901",
                    TelephoneNumber = "1234567890",
                    Email = "person1@example.com",
                    UserId = 1,
                    ProfilePhotoPath = "mock/path/photo1.jpg"
                },
                new Person
                {
                    Id = 2,
                    Name = "Person2",
                    LastName = "LastName2",
                    Gender = Gender.Female,
                    Birthday = new DateTime(1995, 1, 1),
                    PersonalCode = "09876543210",
                    TelephoneNumber = "0987654321",
                    Email = "person2@example.com",
                    UserId = 2,
                    ProfilePhotoPath = "mock/path/photo1.jpg"
                }
            };

            var placesOfResidence = new List<PlaceOfResidence>
            {
                new PlaceOfResidence
                {
                    Id = 1,
                    City = "City1",
                    Street = "Street1",
                    HouseNumber = "1A",
                    ApartmentNumber = "101",
                    PersonId = 1
                },
                new PlaceOfResidence
                {
                    Id = 2,
                    City = "City2",
                    Street = "Street2",
                    HouseNumber = "2B",
                    ApartmentNumber = "202",
                    PersonId = 2
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.Persons.AddRangeAsync(persons);
            await _context.PlacesOfResidence.AddRangeAsync(placesOfResidence);
            await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldReturnPersonDTO_WhenPersonIsCreated()
        {
            // Arrange
            var personCreateDTO = new PersonCreateDTO
            {
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "34567890123",
                TelephoneNumber = "555555555",
                Email = "user3@test.com",
                ProfilePhoto = null,
                PlaceOfResidence = new PlaceOfResidenceCreateDTO
                {
                    City = "City3",
                    Street = "Street3",
                    HouseNumber = "3",
                    ApartmentNumber = "3C"
                }
            };
            _mockPersonRepository.Setup(repo => repo.PersonalCodeExistsForUserAsync(It.IsAny<int>(), personCreateDTO.PersonalCode)).ReturnsAsync(false);
            _mockPersonRepository.Setup(repo => repo.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockPersonRepository.Setup(repo => repo.CreateAsync(It.IsAny<Person>())).Returns(Task.CompletedTask).Callback<Person>(p => p.Id = 3);

            // Act
            var result = await _personService.CreatePersonAsync(1, personCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("User3", result.Name);
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldThrowArgumentException_WhenPersonalCodeExists()
        {
            // Arrange
            var personCreateDTO = new PersonCreateDTO
            {
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "12345678901",
                TelephoneNumber = "555555555",
                Email = "user3@test.com",
                ProfilePhoto = null,
                PlaceOfResidence = new PlaceOfResidenceCreateDTO
                {
                    City = "City3",
                    Street = "Street3",
                    HouseNumber = "3",
                    ApartmentNumber = "3C"
                }
            };
            _mockPersonRepository.Setup(repo => repo.PersonalCodeExistsForUserAsync(It.IsAny<int>(), personCreateDTO.PersonalCode)).ReturnsAsync(true);

            // Act
            async Task Action() => await _personService.CreatePersonAsync(1, personCreateDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(Action);
            Assert.Equal("A person with this Personal Identification Code already exists.", ex.Message);
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldThrowArgumentException_WhenEmailExists()
        {
            // Arrange
            var personCreateDTO = new PersonCreateDTO
            {
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "34567890123",
                TelephoneNumber = "555555555",
                Email = "user3@test.com",
                ProfilePhoto = null,
                PlaceOfResidence = new PlaceOfResidenceCreateDTO
                {
                    City = "City3",
                    Street = "Street3",
                    HouseNumber = "3",
                    ApartmentNumber = "3C"
                }
            };
            _mockPersonRepository.Setup(repo => repo.PersonalCodeExistsForUserAsync(It.IsAny<int>(), personCreateDTO.PersonalCode)).ReturnsAsync(false);
            _mockPersonRepository.Setup(repo => repo.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            async Task Action() => await _personService.CreatePersonAsync(1, personCreateDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(Action);
            Assert.Equal("A person with this email already exists.", ex.Message);
        }

        [Fact]
        public async Task GetAllPersonsByUserIdAsync_ShouldReturnListOfPersonDTOs_WhenPersonsExist()
        {
            // Arrange
            _mockPersonRepository.Setup(repo => repo.GetAllPersonsByUserIdAsync(It.IsAny<int>())).ReturnsAsync(_context.Persons.ToList());

            // Act
            var result = await _personService.GetAllPersonsByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_context.Persons.Count(), result.Count());
        }

        [Fact]
        public async Task GetPersonByIdAsync_ShouldReturnPersonDTO_WhenPersonExists()
        {
            // Arrange
            var personId = 1;
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            var result = await _personService.GetPersonByIdAsync(1, personId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.Name, result.Name);
        }

        [Fact]
        public async Task GetPersonByIdAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
        {
            // Arrange
            var personId = 3;
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync((Person)null);

            // Act
            async Task Action() => await _personService.GetPersonByIdAsync(1, personId);

            // Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(Action);
            Assert.Equal("Person not found.", ex.Message);
        }

        [Fact]
        public async Task GetPersonByIdAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var personId = 1;
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            async Task Action() => await _personService.GetPersonByIdAsync(2, personId);

            // Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(Action);
            Assert.Equal("Access is denied.", ex.Message);
        }

        [Fact]
        public async Task DeletePersonAsync_ShouldReturnPersonDTO_WhenPersonIsDeleted()
        {
            // Arrange
            var personId = 1;
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);
            _mockPersonRepository.Setup(repo => repo.DeleteAsync(personId)).Returns(Task.CompletedTask);

            // Act
            var result = await _personService.DeletePersonAsync(1, personId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.Name, result.Name);
        }

        [Fact]
        public async Task DeletePersonAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
        {
            // Arrange
            var personId = 3;
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync((Person)null);

            // Act
            async Task Action() => await _personService.DeletePersonAsync(1, personId);

            // Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(Action);
            Assert.Equal("Person not found.", ex.Message);
        }

        [Fact]
        public async Task UpdatePersonDetailsAsync_ShouldReturnPersonDTO_WhenPersonDetailsAreUpdated()
        {
            // Arrange
            var personId = 1;
            var personUpdateDetailsDTO = new PersonUpdateDetailsDTO
            {
                Name = "User1 Updated",
                LastName = "Test1 Updated",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "123456789",
                Email = "user1.updated@test.com",
                PlaceOfResidence = new PlaceOfResidenceUpdateDTO
                {
                    City = "Updated City",
                    Street = "Updated Street",
                    HouseNumber = "1",
                    ApartmentNumber = "1A"
                }
            };
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            var result = await _personService.UpdatePersonDetailsAsync(1, personId, personUpdateDetailsDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personUpdateDetailsDTO.Name, result.Name);
            Assert.Equal(personUpdateDetailsDTO.LastName, result.LastName);
        }

        [Fact]
        public async Task UpdatePersonDetailsAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
        {
            // Arrange
            var personId = 3;
            var personUpdateDetailsDTO = new PersonUpdateDetailsDTO
            {
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "34567890123",
                TelephoneNumber = "555555555",
                Email = "user3@test.com",
                PlaceOfResidence = new PlaceOfResidenceUpdateDTO
                {
                    City = "City3",
                    Street = "Street3",
                    HouseNumber = "3",
                    ApartmentNumber = "3C"
                }
            };
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync((Person)null);

            // Act
            async Task Action() => await _personService.UpdatePersonDetailsAsync(1, personId, personUpdateDetailsDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(Action);
            Assert.Equal("Person not found.", ex.Message);
        }

        [Fact]
        public async Task UpdatePersonDetailsAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var personId = 1;
            var personUpdateDetailsDTO = new PersonUpdateDetailsDTO
            {
                Name = "User1",
                LastName = "Test1",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "123456789",
                Email = "user1@test.com",
                PlaceOfResidence = new PlaceOfResidenceUpdateDTO
                {
                    City = "City1",
                    Street = "Street1",
                    HouseNumber = "1",
                    ApartmentNumber = "1A"
                }
            };

            var person = new Person
            {
                Id = personId,
                Name = "User1",
                LastName = "Test1",
                Gender = Gender.Male,
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "123456789",
                Email = "user1@test.com",
                UserId = 1,
                PlaceOfResidence = new PlaceOfResidence
                {
                    City = "City1",
                    Street = "Street1",
                    HouseNumber = "1",
                    ApartmentNumber = "1A"
                }
            };

            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            async Task Action() => await _personService.UpdatePersonDetailsAsync(2, personId, personUpdateDetailsDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(Action);
            Assert.Equal("Access is denied.", ex.Message);
        }

        [Fact]
        public async Task UpdatePersonImageAsync_ShouldReturnPersonDTO_WhenPersonImageIsUpdated()
        {
            // Arrange
            var personId = 1;
            var personUpdateImageDTO = new PersonUpdateImageDTO
            {
                ProfilePhoto = FormFileHelper.CreateMockImageFormFile()
            };
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            var result = await _personService.UpdatePersonImageAsync(1, personId, personUpdateImageDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.Name, result.Name);
        }

        [Fact]
        public async Task UpdatePersonImageAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
        {
            // Arrange
            var personId = 3;
            var personUpdateImageDTO = new PersonUpdateImageDTO
            {
                ProfilePhoto = FormFileHelper.CreateMockImageFormFile()
            };
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync((Person)null);

            // Act
            async Task Action() => await _personService.UpdatePersonImageAsync(1, personId, personUpdateImageDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(Action);
            Assert.Equal("Person not found.", ex.Message);
        }

        [Fact]
        public async Task UpdatePersonImageAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var personId = 1;
            var personUpdateImageDTO = new PersonUpdateImageDTO
            {
                ProfilePhoto = FormFileHelper.CreateMockImageFormFile()
            };
            var person = _context.Persons.First();
            _mockPersonRepository.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act
            async Task Action() => await _personService.UpdatePersonImageAsync(2, personId, personUpdateImageDTO);

            // Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(Action);
            Assert.Equal("Access is denied.", ex.Message);
        }
    }
}

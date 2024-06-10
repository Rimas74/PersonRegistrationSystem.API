using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.API.Controllers;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.BusinessLogic.Services;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Enums;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UnitTestHelpers.Helpers;
using Xunit;

namespace UnitTestAPI
{
    public class PersonControllerUnitTests : IAsyncLifetime
    {
        private readonly Mock<ILogger<PersonService>> _mockServiceLogger;
        private readonly Mock<ILogger<PersonController>> _mockControllerLogger;
        private readonly Mock<ILogger<PersonRepository>> _mockRepositoryLogger;
        private readonly PersonController _controller;
        private PersonRegistrationContext _context;
        private PersonService _personService;
        private IPersonRepository _personRepository;

        public PersonControllerUnitTests()
        {
            _mockServiceLogger = new Mock<ILogger<PersonService>>();
            _mockControllerLogger = new Mock<ILogger<PersonController>>();
            _mockRepositoryLogger = new Mock<ILogger<PersonRepository>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonDTO>().ReverseMap();
                cfg.CreateMap<PersonCreateDTO, Person>();
                cfg.CreateMap<PersonUpdateDetailsDTO, Person>();
                cfg.CreateMap<PersonUpdateImageDTO, Person>();
                cfg.CreateMap<PlaceOfResidenceCreateDTO, PlaceOfResidence>();
                cfg.CreateMap<PlaceOfResidenceUpdateDTO, PlaceOfResidence>();
                cfg.CreateMap<PlaceOfResidence, PlaceOfResidenceDTO>().ReverseMap();
            });
            var mapper = mapperConfig.CreateMapper();

            _context = GetDatabaseContext().Result;
            _personRepository = new PersonRepository(_context, _mockRepositoryLogger.Object);
            _personService = new PersonService(_personRepository, mapper, _mockServiceLogger.Object);
            _controller = new PersonController(_personService, _mockControllerLogger.Object);
        }

        public async Task InitializeAsync()
        {
            await SeedData();
        }

        private async Task<PersonRegistrationContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<PersonRegistrationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new PersonRegistrationContext(options);
            await context.Database.EnsureCreatedAsync();
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
                    ProfilePhotoPath = "mock/path/photo2.jpg"
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

        private void SetUserClaims(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetAllPerson_ShouldReturnOk_WhenPersonsExist()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);
            var expectedPersons = new List<PersonDTO>
            {
                new PersonDTO
                {
                    Id = 1,
                    Name = "Person1",
                    LastName = "LastName1",
                    Gender = "Male",
                    Birthday = new DateTime(1990, 1, 1),
                    PersonalCode = "12345678901",
                    TelephoneNumber = "1234567890",
                    Email = "person1@example.com"
                }
            };

            // Act
            var result = await _controller.GetAllPerson();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var actualPersons = Assert.IsType<List<PersonDTO>>(okResult.Value);

            Assert.Equal(expectedPersons.Count, actualPersons.Count);
            for (int i = 0; i < expectedPersons.Count; i++)
            {
                Assert.Equal(expectedPersons[i].Name, actualPersons[i].Name);
                Assert.Equal(expectedPersons[i].LastName, actualPersons[i].LastName);
                Assert.Equal(expectedPersons[i].Gender, actualPersons[i].Gender);
                Assert.Equal(expectedPersons[i].Birthday, actualPersons[i].Birthday);
                Assert.Equal(expectedPersons[i].PersonalCode, actualPersons[i].PersonalCode);
                Assert.Equal(expectedPersons[i].TelephoneNumber, actualPersons[i].TelephoneNumber);
                Assert.Equal(expectedPersons[i].Email, actualPersons[i].Email);
            }
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnOk_WhenPersonExists()
        {
            // Arrange
            var userId = 1;
            var personId = 1;
            SetUserClaims(userId);

            var expectedPerson = new PersonDTO
            {
                Id = personId,
                Name = "Person1",
                LastName = "LastName1",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "1234567890",
                Email = "person1@example.com"
            };

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var actualPerson = Assert.IsType<PersonDTO>(okResult.Value);

            Assert.Equal(expectedPerson.Name, actualPerson.Name);
            Assert.Equal(expectedPerson.LastName, actualPerson.LastName);
            Assert.Equal(expectedPerson.Gender, actualPerson.Gender);
            Assert.Equal(expectedPerson.Birthday, actualPerson.Birthday);
            Assert.Equal(expectedPerson.PersonalCode, actualPerson.PersonalCode);
            Assert.Equal(expectedPerson.TelephoneNumber, actualPerson.TelephoneNumber);
            Assert.Equal(expectedPerson.Email, actualPerson.Email);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var personId = 99;
            SetUserClaims(userId);

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Person not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreated_WhenPersonIsCreated()
        {
            // Arrange
            var userId = 1;
            SetUserClaims(userId);

            var personCreateDTO = new PersonCreateDTO
            {
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "34567890123",
                TelephoneNumber = "555555555",
                Email = "user3@test.com",
                ProfilePhoto = FormFileHelper.CreateMockImageFormFile(),
                PlaceOfResidence = new PlaceOfResidenceCreateDTO
                {
                    City = "City3",
                    Street = "Street3",
                    HouseNumber = "3",
                    ApartmentNumber = "3C"
                }
            };

            var createdPersonDTO = new PersonDTO
            {
                Id = 3,
                Name = "User3",
                LastName = "Test3",
                Gender = "Female",
                Birthday = new DateTime(1995, 5, 5),
                PersonalCode = "34567890123",
                TelephoneNumber = "555555555",
                Email = "user3@test.com"
            };

            // Act
            var result = await _controller.CreatePerson(personCreateDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdAtActionResult);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            var actualPerson = Assert.IsType<PersonDTO>(createdAtActionResult.Value);
            Assert.Equal(createdPersonDTO.Name, actualPerson.Name);
            Assert.Equal(createdPersonDTO.LastName, actualPerson.LastName);
            Assert.Equal(createdPersonDTO.Gender, actualPerson.Gender);
            Assert.Equal(createdPersonDTO.Birthday, actualPerson.Birthday);
            Assert.Equal(createdPersonDTO.PersonalCode, actualPerson.PersonalCode);
            Assert.Equal(createdPersonDTO.TelephoneNumber, actualPerson.TelephoneNumber);
            Assert.Equal(createdPersonDTO.Email, actualPerson.Email);
        }

        [Fact]
        public async Task UpdatePersonDetails_ShouldReturnOk_WhenPersonDetailsAreUpdated()
        {
            // Arrange
            var userId = 1;
            var personId = 1;
            SetUserClaims(userId);

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

            var updatedPersonDTO = new PersonDTO
            {
                Id = personId,
                Name = "User1 Updated",
                LastName = "Test1 Updated",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "123456789",
                Email = "user1.updated@test.com"
            };

            // Act
            var result = await _controller.UpdatePersonDetails(personId, personUpdateDetailsDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var actualPerson = Assert.IsType<PersonDTO>(okResult.Value);
            Assert.Equal(updatedPersonDTO.Name, actualPerson.Name);
            Assert.Equal(updatedPersonDTO.LastName, actualPerson.LastName);
            Assert.Equal(updatedPersonDTO.Gender, actualPerson.Gender);
            Assert.Equal(updatedPersonDTO.Birthday, actualPerson.Birthday);
            Assert.Equal(updatedPersonDTO.PersonalCode, actualPerson.PersonalCode);
            Assert.Equal(updatedPersonDTO.TelephoneNumber, actualPerson.TelephoneNumber);
            Assert.Equal(updatedPersonDTO.Email, actualPerson.Email);
        }

        [Fact]
        public async Task UpdatePersonImage_ShouldReturnOk_WhenPersonImageIsUpdated()
        {
            // Arrange
            var userId = 1;
            var personId = 1;
            SetUserClaims(userId);

            var personUpdateImageDTO = new PersonUpdateImageDTO
            {
                ProfilePhoto = FormFileHelper.CreateMockImageFormFile()
            };

            var updatedPersonDTO = new PersonDTO
            {
                Id = personId,
                Name = "Person1",
                LastName = "LastName1",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "1234567890",
                Email = "person1@example.com"
            };

            // Act
            var result = await _controller.UpdatePersonImage(personId, personUpdateImageDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var actualPerson = Assert.IsType<PersonDTO>(okResult.Value);
            Assert.Equal(updatedPersonDTO.Name, actualPerson.Name);
            Assert.Equal(updatedPersonDTO.LastName, actualPerson.LastName);
            Assert.Equal(updatedPersonDTO.Gender, actualPerson.Gender);
            Assert.Equal(updatedPersonDTO.Birthday, actualPerson.Birthday);
            Assert.Equal(updatedPersonDTO.PersonalCode, actualPerson.PersonalCode);
            Assert.Equal(updatedPersonDTO.TelephoneNumber, actualPerson.TelephoneNumber);
            Assert.Equal(updatedPersonDTO.Email, actualPerson.Email);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnOk_WhenPersonIsDeleted()
        {
            // Arrange
            var userId = 1;
            var personId = 1;
            SetUserClaims(userId);

            var deletedPersonDTO = new PersonDTO
            {
                Id = personId,
                Name = "Person1",
                LastName = "LastName1",
                Gender = "Male",
                Birthday = new DateTime(1990, 1, 1),
                PersonalCode = "12345678901",
                TelephoneNumber = "1234567890",
                Email = "person1@example.com"
            };

            // Act
            var result = await _controller.DeletePerson(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var actualPerson = Assert.IsType<PersonDTO>(okResult.Value);
            Assert.Equal(deletedPersonDTO.Name, actualPerson.Name);
            Assert.Equal(deletedPersonDTO.LastName, actualPerson.LastName);
            Assert.Equal(deletedPersonDTO.Gender, actualPerson.Gender);
            Assert.Equal(deletedPersonDTO.Birthday, actualPerson.Birthday);
            Assert.Equal(deletedPersonDTO.PersonalCode, actualPerson.PersonalCode);
            Assert.Equal(deletedPersonDTO.TelephoneNumber, actualPerson.TelephoneNumber);
            Assert.Equal(deletedPersonDTO.Email, actualPerson.Email);
        }
    }
}

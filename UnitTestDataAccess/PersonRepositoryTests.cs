using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.DataAccess;
using Xunit.Abstractions;

namespace UnitTestDataAccess
{
    public class PersonRepositoryTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private PersonRegistrationContext _context;
        private PersonRepository _personRepository;
        private Mock<ILogger<PersonRepository>> _mockLogger;

        public PersonRepositoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public async Task InitializeAsync()
        {
            _mockLogger = new Mock<ILogger<PersonRepository>>();
            _context = await GetDatabaseContext();
            _personRepository = new PersonRepository(_context, _mockLogger.Object);

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
                    Gender = PersonRegistrationSystem.DataAccess.Enums.Gender.Male,
                    Birthday = new DateTime(1990, 1, 1),
                    PersonalCode = "12345678901",
                    TelephoneNumber = "1234567890",
                    Email = "person1@example.com",
                    UserId = 1,
                    ProfilePhotoPath = "path/to/photo1.jpg"
                },
                new Person
                {
                    Id = 2,
                    Name = "Person2",
                    LastName = "LastName2",
                    Gender = PersonRegistrationSystem.DataAccess.Enums.Gender.Female,
                    Birthday = new DateTime(1995, 1, 1),
                    PersonalCode = "09876543210",
                    TelephoneNumber = "0987654321",
                    Email = "person2@example.com",
                    UserId = 2,
                    ProfilePhotoPath = "path/to/photo2.jpg"
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
        public async Task AddPerson_ShouldReturnPerson()
        {
            // Arrange
            var newPerson = new Person
            {
                Id = 3,
                Name = "Person3",
                LastName = "LastName3",
                Gender = PersonRegistrationSystem.DataAccess.Enums.Gender.Male,
                Birthday = new DateTime(2000, 1, 1),
                PersonalCode = "11223344556",
                TelephoneNumber = "111222333",
                Email = "person3@example.com",
                UserId = 1,
                ProfilePhotoPath = "path/to/photo3.jpg"
            };

            // Act
            await _personRepository.CreateAsync(newPerson);
            var person = await _personRepository.GetByIdAsync(3);

            // Assert
            Assert.NotNull(person);
            Assert.Equal(3, person.Id);
            Assert.Equal("Person3", person.Name);
        }

        [Fact]
        public async Task DeletePerson_ShouldRemovePerson_WhenPersonExists()
        {
            // Act
            await _personRepository.DeleteAsync(1);
            var person = await _personRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(person);
        }

        [Fact]
        public async Task UpdatePerson_ShouldUpdatePersonDetails_WhenPersonExists()
        {
            // Arrange
            var existingPerson = await _personRepository.GetByIdAsync(1);
            existingPerson.Name = "Person1Updated";

            // Act
            await _personRepository.UpdatePersonDetailsAsync(existingPerson);
            var updatedPerson = await _personRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(updatedPerson);
            Assert.Equal("Person1Updated", updatedPerson.Name);
        }

        [Fact]
        public async Task GetAllPersonsByUserId_ShouldReturnAllPersonsForUser()
        {
            // Act
            var persons = await _personRepository.GetAllPersonsByUserIdAsync(1);

            // Assert
            Assert.NotNull(persons);
            Assert.Single(persons);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnPerson_WhenPersonExists()
        {
            // Act
            var person = await _personRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(person);
            Assert.Equal(1, person.Id);
            Assert.Equal("Person1", person.Name);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnNull_WhenPersonDoesNotExist()
        {
            // Act
            var person = await _personRepository.GetByIdAsync(3);

            // Assert
            Assert.Null(person);
        }

        [Fact]
        public async Task PersonalCodeExistsForUser_ShouldReturnTrue_WhenPersonalCodeExists()
        {
            // Act
            var exists = await _personRepository.PersonalCodeExistsForUserAsync(1, "12345678901");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task PersonalCodeExistsForUser_ShouldReturnFalse_WhenPersonalCodeDoesNotExist()
        {
            // Act
            var exists = await _personRepository.PersonalCodeExistsForUserAsync(1, "00000000000");

            // Assert
            Assert.False(exists);
        }
        [Fact]
        public async Task EmailExists_ShouldReturnTrue_WhenEmailExists()
        {
            // Act
            var exists = await _personRepository.EmailExistsAsync("person1@example.com");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task EmailExists_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Act
            var exists = await _personRepository.EmailExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(exists);
        }
    }
}

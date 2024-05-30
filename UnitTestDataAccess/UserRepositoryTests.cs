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
    public class UserRepositoryTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private PersonRegistrationContext _context;
        private UserRepository _userRepository;
        private Mock<ILogger<UserRepository>> _mockLogger;

        public UserRepositoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public async Task InitializeAsync()
        {
            _mockLogger = new Mock<ILogger<UserRepository>>();
            _context = await GetDatabaseContext();
            _userRepository = new UserRepository(_context, _mockLogger.Object);
        }

        private async Task<PersonRegistrationContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<PersonRegistrationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new PersonRegistrationContext(options);
            await context.Database.EnsureCreatedAsync();

            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            if (!context.Users.Any())
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

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // Log the seeded users
            var allUsers = await context.Users.ToListAsync();
            _output.WriteLine("Seeded Users:");
            foreach (var user in allUsers)
            {
                _output.WriteLine($"Id: {user.Id}, Username: {user.Username}, Role: {user.Role}");
            }

            return context;
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task AddUser_ShouldReturnUser()
        {
            // Arrange
            var newUser = new User
            {
                Id = 3,
                Username = "user3",
                PasswordHash = "hashedpassword3",
                Salt = "salt3",
                Role = "User"
            };

            // Act
            await _userRepository.AddAsync(newUser);
            var user = await _userRepository.GetByIdAsync(3);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(3, user.Id);
            Assert.Equal("user3", user.Username);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
        {
            // Act
            await _userRepository.DeleteAsync(1);
            var user = await _userRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUserDetails_WhenUserExists()
        {
            // Arrange
            var existingUser = await _userRepository.GetByIdAsync(1);
            existingUser.Username = "user1Updated";

            // Act
            await _userRepository.UpdateAsync(existingUser);
            var updatedUser = await _userRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("user1Updated", updatedUser.Username);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Act
            var users = await _userRepository.GetAllAsync();

            // Log the users retrieved
            _output.WriteLine("Retrieved Users:");
            foreach (var user in users)
            {
                _output.WriteLine($"Id: {user.Id}, Username: {user.Username}, Role: {user.Role}");
            }

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Act
            var user = await _userRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("user1", user.Username);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var user = await _userRepository.GetByIdAsync(3);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
        {
            // Act
            var user = await _userRepository.GetByUsernameAsync("user2");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("user2", user.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnNull_WhenUsernameDoesNotExist()
        {
            // Act
            var user = await _userRepository.GetByUsernameAsync("NonExistentUser");

            // Assert
            Assert.Null(user);
        }
    }
}

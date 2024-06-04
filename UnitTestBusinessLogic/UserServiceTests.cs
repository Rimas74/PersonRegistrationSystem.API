using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.BusinessLogic.Services;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;
using PersonRegistrationSystem.DataAccess.Interfaces;
using Xunit;

namespace UnitTestBusinessLogic
{
    public class UserServiceTests : IAsyncLifetime
    {
        private PersonRegistrationContext _context;
        private UserService _userService;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ITokenService> _mockTokenService;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UserService>> _mockLogger;

        private List<User> _users;
        private List<UserDTO> _userDTOs;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
        }

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
            _userService = new UserService(_mockUserRepository.Object, _mockTokenService.Object, _mockMapper.Object, _mockLogger.Object);

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
            await context.SaveChangesAsync();

            return context;
        }

        private async Task SeedData()
        {
            var (passwordHash1, salt1) = PasswordHasher.CreatePasswordHash("Password123!@#");
            var (passwordHash2, salt2) = PasswordHasher.CreatePasswordHash("Password456!@#");

            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "user1",
                    PasswordHash = passwordHash1,
                    Salt = salt1,
                    Role = "User"
                },
                new User
                {
                    Id = 2,
                    Username = "user2",
                    PasswordHash = passwordHash2,
                    Salt = salt2,
                    Role = "Admin"
                }
            };

            _userDTOs = new List<UserDTO>
            {
                new UserDTO { Id = 1, Username = "user1" },
                new UserDTO { Id = 2, Username = "user2" }
            };

            await _context.Users.AddRangeAsync(_users);
            await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnUserDTO()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO { Username = "user3", Password = "Password123!@#" };
            var (passwordHash, salt) = PasswordHasher.CreatePasswordHash("Password123!@#");
            var newUser = new User { Id = 3, Username = "user3", PasswordHash = passwordHash, Salt = salt, Role = "User" };
            var userDTO = new UserDTO { Id = 3, Username = "user3" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(newUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            // Act
            var result = await _userService.RegisterUserAsync(userRegisterDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user3", result.Username);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnTokenDTO()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "Password123!@#" };
            var user = _users.FirstOrDefault(u => u.Username == "user1");
            var tokenDTO = new TokenDTO { Token = "token" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns("token");

            // Act
            var result = await _userService.LoginUserAsync(userLoginDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("token", result.Token);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldThrowUnauthorizedAccessException_ForInvalidPassword()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "WrongPassword!@#" };
            var user = _users.FirstOrDefault(u => u.Username == "user1");

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginUserAsync(userLoginDTO));

            // Assert

            Assert.Equal("Invalid password.", exception.Message);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnUserDTO()
        {
            // Arrange
            var userId = 1;
            var user = _users.First(u => u.Id == userId);
            var userDTO = _userDTOs.First(u => u.Id == userId);

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1", result.Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDTO>>(It.IsAny<IEnumerable<User>>())).Returns(_userDTOs);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}

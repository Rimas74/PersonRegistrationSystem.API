using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.BusinessLogic.Helpers;
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
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;
        private PersonRegistrationContext _context;
        private List<User> _users;
        private List<UserDTO> _userDTOs;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPersonRepository = new Mock<IPersonRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<UserService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();
                cfg.CreateMap<UserRegisterDTO, User>();
                cfg.CreateMap<UserLoginDTO, User>();
            });

            _mapper = config.CreateMapper();
            _userService = new UserService(_mockUserRepository.Object, _mockPersonRepository.Object, _mockTokenService.Object, _mapper, _mockLogger.Object);
        }

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
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
            var (passwordHash1, salt1) = PasswordHasher.CreatePasswordHash("Password12!@");
            var (passwordHash2, salt2) = PasswordHasher.CreatePasswordHash("Password45!@");

            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "user1",
                    PasswordHash = passwordHash1,
                    Salt = salt1,
                    Role = "User",
                    Persons = new List<Person>()
                },
                new User
                {
                    Id = 2,
                    Username = "user2",
                    PasswordHash = passwordHash2,
                    Salt = salt2,
                    Role = "Admin",
                    Persons = new List<Person>()
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
        public async Task RegisterUserAsync_ShouldReturnUserDTO_WhenUserIsRegistered()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO { Username = "user3", Password = "Password12!@" };
            var user = new User { Id = 3, Username = "user3", PasswordHash = "hashedpassword3", Salt = "salt3" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(userRegisterDTO.Username)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask).Callback<User>(u => u.Id = 3);

            // Act
            var result = await _userService.RegisterUserAsync(userRegisterDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("user3", result.Username);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowArgumentException_WhenUsernameExists()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO { Username = "user1", Password = "Password12!@" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(userRegisterDTO.Username)).ReturnsAsync(_users.First());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _userService.RegisterUserAsync(userRegisterDTO));
            Assert.Equal("Username already exists.", ex.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnTokenDTO_WhenCredentialsAreValid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "Password12!@" };
            var user = _users.First();
            var token = "generatedToken";

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(userLoginDTO.Username)).ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateToken(user.Username, user.Role, user.Id)).Returns(token);

            // Act
            var result = await _userService.LoginUserAsync(userLoginDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "WrongPassword!@" };
            var user = _users.First();

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(userLoginDTO.Username)).ReturnsAsync(user);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginUserAsync(userLoginDTO));
            Assert.Equal("Invalid username or password.", ex.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldThrowUnauthorizedAccessException_WhenUsernameIsInvalid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "invalidUser", Password = "Password12!@" };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(userLoginDTO.Username)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginUserAsync(userLoginDTO));
            Assert.Equal("Invalid username or password.", ex.Message);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnUserDTO_WhenUserIsDeleted()
        {
            // Arrange
            var userId = 1;
            var user = _users.First();
            var userDTO = _userDTOs.First();

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteUserAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDTO.Id, result.Id);
            Assert.Equal(userDTO.Username, result.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 3;

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteUserAsync(userId));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUserDTOs_WhenUsersExist()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_users.Count, result.Count());
        }
    }
}

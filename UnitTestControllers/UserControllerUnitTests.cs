using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonRegistrationSystem.API.Controllers;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;
using Xunit;

namespace UnitTestAPI
{
    public class UserControllerTests : IAsyncLifetime
    {
        private PersonRegistrationContext _context;
        private UserController _userController;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<UserController>> _mockLogger;

        private List<User> _users;
        private List<UserDTO> _userDTOs;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
        }

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
            _userController = new UserController(_mockUserService.Object, _mockLogger.Object);

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
                new UserDTO { Id = 1, Username = "user1", Role = "User" },
                new UserDTO { Id = 2, Username = "user2", Role = "Admin" }
            };

            await _context.Users.AddRangeAsync(_users);
            await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Signup_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO { Username = "user3", Password = "Password123!@#" };
            var userDTO = new UserDTO { Id = 3, Username = "user3", Role = "User" };

            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<UserRegisterDTO>())).ReturnsAsync(userDTO);

            // Act
            var result = await _userController.Signup(userRegisterDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userDTO, okResult.Value);
        }

        [Fact]
        public async Task Signup_ShouldReturnBadRequest_WhenUsernameExists()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO { Username = "user1", Password = "Password123!@#" };

            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<UserRegisterDTO>())).ThrowsAsync(new ArgumentException("Username already exists."));

            // Act
            var result = await _userController.Signup(userRegisterDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Username already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "Password123!@#" };
            var tokenDTO = new TokenDTO { Token = "token" };

            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<UserLoginDTO>())).ReturnsAsync(tokenDTO);

            // Act
            var result = await _userController.Login(userLoginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(tokenDTO, okResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "user1", Password = "WrongPassword!@#" };

            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<UserLoginDTO>())).ThrowsAsync(new UnauthorizedAccessException("Invalid password."));

            // Act
            var result = await _userController.Login(userLoginDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal("Invalid password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUsernameIsInvalid()
        {
            // Arrange
            var userLoginDTO = new UserLoginDTO { Username = "invalidUser", Password = "Password123!@#" };

            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<UserLoginDTO>())).ThrowsAsync(new UnauthorizedAccessException("Invalid username."));

            // Act
            var result = await _userController.Login(userLoginDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal("Invalid username.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WhenUsersExist_AndUserIsAdmin()
        {
            // Arrange
            SetUserRole("Admin");
            _mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(_userDTOs);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(_userDTOs, okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            SetUserRole("User");

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            SetUserRole("Admin");
            _mockUserService.Setup(service => service.GetAllUsersAsync()).ThrowsAsync(new Exception("Error getting all users"));

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Error getting all users", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenUserIsDeleted_AndUserIsAdmin()
        {
            // Arrange
            SetUserRole("Admin");
            var userId = 1;
            var userDTO = _userDTOs.First(u => u.Id == userId);

            _mockUserService.Setup(service => service.DeleteUserAsync(It.IsAny<int>())).ReturnsAsync(userDTO);

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userDTO, okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            SetUserRole("User");
            var userId = 1;

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnBadRequest_WhenUserDoesNotExist_AndUserIsAdmin()
        {
            // Arrange
            SetUserRole("Admin");
            var userId = 3;

            _mockUserService.Setup(service => service.DeleteUserAsync(It.IsAny<int>())).ThrowsAsync(new ArgumentException("User not found."));

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("User not found.", badRequestResult.Value);
        }
    }
}

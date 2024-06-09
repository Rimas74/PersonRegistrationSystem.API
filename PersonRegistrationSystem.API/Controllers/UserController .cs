using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("Signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(UserRegisterDTO userRegisterDTO)
        {
            _logger.LogInformation($"Registering user: {userRegisterDTO.Username}");
            try
            {
                var user = await _userService.RegisterUserAsync(userRegisterDTO);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Error registering user: {userRegisterDTO.Username}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            _logger.LogInformation($"Logging in user: {userLoginDTO.Username}");
            try
            {
                var tokenDTO = await _userService.LoginUserAsync(userLoginDTO);
                return Ok(tokenDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, $"Error logging in user: {userLoginDTO.Username}");
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _logger.LogInformation("Getting all users");
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                _logger.LogWarning($"User does not have the required role to delete user ID: {id}");
                return Forbid();
            }

            try
            {
                var deletedUser = await _userService.DeleteUserAsync(id);
                if (deletedUser == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(deletedUser);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the user with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}

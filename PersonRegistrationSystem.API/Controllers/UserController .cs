﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _logger.LogInformation($"Deleting user with ID: {userId}");
            try
            {
                var user = await _userService.DeleteUserAsync(userId);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID: {userId}");
                return BadRequest(ex.Message);
            }
        }
    }
}

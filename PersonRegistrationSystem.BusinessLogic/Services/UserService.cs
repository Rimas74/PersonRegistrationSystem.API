using AutoMapper;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Interfaces;
using PersonRegistrationSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PersonRegistrationSystem.DataAccess.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IPersonRepository personRepository, ITokenService tokenService, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDTO> RegisterUserAsync(UserRegisterDTO userRegisterDTO)
        {
            _logger.LogInformation($"Registering user with username: {userRegisterDTO.Username}");

            var existingUser = await _userRepository.GetByUsernameAsync(userRegisterDTO.Username);
            if (existingUser != null)
            {
                _logger.LogWarning($"Username {userRegisterDTO.Username} already exists.");
                throw new ArgumentException("Username already exists.");
            }

            var user = _mapper.Map<User>(userRegisterDTO);

            var (passwordHash, salt) = PasswordHasher.CreatePasswordHash(userRegisterDTO.Password);


            user.PasswordHash = passwordHash;
            user.Salt = salt;

            await _userRepository.AddAsync(user);

            _logger.LogInformation($"User {userRegisterDTO.Username} registered successfully.");
            return _mapper.Map<UserDTO>(user);
        }


        public async Task<TokenDTO> LoginUserAsync(UserLoginDTO userLoginDTO)
        {
            _logger.LogInformation($"Logging in user with username: {userLoginDTO.Username}");

            var user = await _userRepository.GetByUsernameAsync(userLoginDTO.Username);
            ValidateUserCredentials(user, userLoginDTO.Password);

            var token = _tokenService.GenerateToken(user.Username, user.Role, user.Id);

            _logger.LogInformation($"User {userLoginDTO.Username} logged in successfully.");
            return new TokenDTO { Token = token };
        }


        public async Task<UserDTO> DeleteUserAsync(int userId)
        {
            _logger.LogInformation($"Deleting user with ID: {userId}");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID: {userId} not found.");
                throw new KeyNotFoundException("User not found.");
            }

            foreach (var person in user.Persons)
            {
                _logger.LogInformation($"Deleting person with ID: {person.Id} for user ID: {userId}");
                await _personRepository.DeleteAsync(person.Id);
            }

            var deletedUser = await _userRepository.DeleteUserAsync(userId);
            _logger.LogInformation($"User with ID: {userId} has been removed from the database.");

            return _mapper.Map<UserDTO>(deletedUser);
        }




        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }


        private void ValidateUserCredentials(User user, string password)
        {
            if (user == null)
            {
                _logger.LogWarning("Invalid username.");
                throw new UnauthorizedAccessException("Invalid username.");

            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordValid)
            {
                _logger.LogWarning($"Invalid password for username {user.Username}.");
                throw new UnauthorizedAccessException("Invalid password.");
            }
        }



    }
}

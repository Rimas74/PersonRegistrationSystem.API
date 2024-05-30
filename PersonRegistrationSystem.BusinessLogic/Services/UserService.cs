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

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserDTO> RegisterUserAsync(UserRegisterDTO userRegisterDTO)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(userRegisterDTO.Username);
            if (existingUser != null)
            {
                throw new ArgumentException("Username already exists.");
            }

            var user = _mapper.Map<User>(userRegisterDTO);

            var (passwordHash, salt) = PasswordHasher.CreatePasswordHash(userRegisterDTO.Password);
            

            user.PasswordHash = passwordHash;
            user.Salt = salt;

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDTO>(user);
        }


        public async Task<TokenDTO> LoginUserAsync(UserLoginDTO userLoginDTO)
        {
            var user = await _userRepository.GetByUsernameAsync(userLoginDTO.Username);
            ValidateUserCredentials(user, userLoginDTO.Password);

            var token = _tokenService.GenerateToken(user.Username, user.Role);
            return new TokenDTO { Token = token };
        }



        public async Task<UserDTO> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            await _userRepository.DeleteAsync(userId);
            return _mapper.Map<UserDTO>(user);
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
                throw new UnauthorizedAccessException("Invalid username.");

            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
        }

        

    }
}

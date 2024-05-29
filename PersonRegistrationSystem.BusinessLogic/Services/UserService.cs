using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Interfaces;
using PersonRegistrationSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public Task<UserDTO> DeleteUSerAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> LoginUserAsync(UserLoginDTO userLoginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> RegisterUserAsync(UserRegisterDTO userRegisterDTO)
        {
            throw new NotImplementedException();
        }
    }
}

using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(UserRegisterDTO userRegisterDTO);
        Task<TokenDTO> LoginUserAsync(UserLoginDTO userLoginDTO);
        Task<UserDTO> DeleteUserAsync(int userId);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    }
}

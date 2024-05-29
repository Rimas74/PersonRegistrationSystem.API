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
        Task<UserDTO> LoginUserAsync(UserLoginDTO userLoginDTO);
        Task<UserDTO> DeleteUSerAsync(int userId);
    }
}

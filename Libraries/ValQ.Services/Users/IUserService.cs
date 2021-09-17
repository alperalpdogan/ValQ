using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.User;
using ValQ.Services.DTO;

namespace ValQ.Services.Users
{
    public interface IUserService
    {
        Task<User> GetUserById(string userId);

        Task<RegisterResult> RegisterUserAsync(RegisterUserDTO user);
    }
}

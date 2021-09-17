using ValQ.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public interface IUserService
    {
        Task<User> GetUserById(string userId);

        Task<CreateUserResult> CreateUserAsync(User user, string plainPassword);

        Task AddUserToRoleAsync(User existingUser, string roleName);

        Task AuthenticateUserAsync(string eMail, string plainPassword);
    }
}

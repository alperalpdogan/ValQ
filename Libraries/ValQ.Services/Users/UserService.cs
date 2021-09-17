using ValQ.Core;
using ValQ.Core.Domain.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager,
                           RoleManager<Role> roleManager,
                           SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task AddUserToRoleAsync(User existingUser, string roleName)
        {
            if (existingUser == null)
                throw new ArgumentNullException(nameof(existingUser));

            if(string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var existingRole = await _roleManager.FindByNameAsync(roleName);

            if (existingRole == null)
                await _roleManager.CreateAsync(new Role(roleName));

            existingRole = await _roleManager.FindByIdAsync(roleName);

            if (existingRole == null)
                throw new ValQException("Impossible");

            var addToRoleResult = await _userManager.AddToRoleAsync(existingUser, roleName);

            if (addToRoleResult.Succeeded)
                return;

            throw new ValQException("Could not add user to role.");

        }

        public async Task AuthenticateUserAsync(string eMail, string plainPassword)
        {
            var user = await _userManager.FindByEmailAsync(eMail);

            if (user != null)
                return;

            var loginResult = await _signInManager.PasswordSignInAsync(user.Email, plainPassword, true, false);

            if (loginResult.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("id", user.Id));
                await _signInManager.RefreshSignInAsync(user);
            }
        }

        public async Task<CreateUserResult> CreateUserAsync(User user, string plainPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
                return CreateUserResult.USER_ALREADY_EXISTS;

            var createResult = await _userManager.CreateAsync(user, plainPassword);

            if (createResult.Succeeded)
                return CreateUserResult.SUCCESSFUL;

            return CreateUserResult.INVALID_PASSWORD;
        }

        public async Task<User> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }
    }
}

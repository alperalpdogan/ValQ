using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Logging;
using ValQ.Core.Domain.Users;
using ValQ.Services.DTO;
using ValQ.Services.Logging;

namespace ValQ.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;

        public UserService(UserManager<User> userManager,
                           ILogger logger,
                           IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        public async Task<User> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<RegisterResult> RegisterUserAsync(RegisterUserDTO user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userExists = (await _userManager.FindByEmailAsync(user.Email)) != null;

            if (userExists)
                return new RegisterResult(RegisterFailureReason.USER_EXISTS);

            var createUserResult = await _userManager.CreateAsync(new User()
            {
                UserName = user.Email,
                Email = user.Email,
                Elo = 1000
            }, user.Password);

            if (!createUserResult.Succeeded)
                await _logger.InsertLogAsync(LogLevel.Error, "Could not create user", string.Concat(createUserResult.Errors.Select(o => o.Description)));

            var authResult = await _authenticationService.AuthenticateAsync(user.Email, user.Password);

            if (!authResult.Succesful)
                throw new ValQException("Impossible");

            return new RegisterResult(authResult.Token);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Configuration;
using ValQ.Core.Domain.User;
using ValQ.Core.Infrastructure;

namespace ValQ.Services.Users
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Ctor
        public AuthenticationService(UserManager<User> userManager,
                                     RoleManager<Role> roleManager,
                                     SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Utilities
        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            return claims;
        }
        #endregion

        #region Methods
        public async Task<AuthenticationResult> AuthenticateAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return new AuthenticationResult(AuthenticationFailureReason.USER_NOT_EXISTS);

            var authResult = await _signInManager.PasswordSignInAsync(userName, password, true, false);

            if (!authResult.Succeeded)
            {
                if (authResult.IsLockedOut)
                    return new AuthenticationResult(AuthenticationFailureReason.LOCKED_OUT);

                if (authResult.IsNotAllowed)
                    return new AuthenticationResult(AuthenticationFailureReason.DELETED);

                return new AuthenticationResult(AuthenticationFailureReason.WRONG_PASSWORD);
            }

            var jwtSettings = Singleton<AppSettings>.Instance.JWTConfig;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SignInSecret);
            //TODO: Remove 30 days 
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(await GetClaims(user)),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.TokenLifeTimeInMinutes).AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return new AuthenticationResult(tokenHandler.WriteToken(tokenHandler.CreateJwtSecurityToken(tokenDescriptor)));
        }
        #endregion

    }
}

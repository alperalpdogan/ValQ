using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Framework.Models;
using ValQ.API.Model.Request;
using ValQ.API.Model.Response;
using ValQ.Services.DTO;
using ValQ.Services.Users;

namespace ValQ.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AccountController(IAuthenticationService authenticationService,
                                 IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), 423)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Login([FromQuery] LoginRequest loginRequest)
        {
            var result = await _authenticationService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

            if (result.Succesful)
                return Ok(new LoginResponse() 
                { 
                    JWT = new JWT()
                    {
                        Token = result.Token
                    }
                });

            if (result.FailureReason == AuthenticationFailureReason.USER_NOT_EXISTS || result.FailureReason == AuthenticationFailureReason.DELETED)
                return NotFound(new Error("User not found"));

            if (result.FailureReason == AuthenticationFailureReason.LOCKED_OUT)
                return StatusCode(423, new Error("Your account has been locked out"));

            if (result.FailureReason == AuthenticationFailureReason.WRONG_PASSWORD)
                return BadRequest(new Error("Invalid password"));

            return BadRequest();
        }

        [HttpPost]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), 409)]
        [ProducesResponseType(typeof(Error), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var result = await _userService.RegisterUserAsync(new RegisterUserDTO()
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            });

            if (result.Successful)
                return Ok(new RegisterResponse()
                {
                    JWT = new JWT()
                    {
                        Token = result.Token
                    }
                });

            if (result.FailureReason == RegisterFailureReason.USER_EXISTS)
                return StatusCode(409, new Error("User already exists"));

            if (result.FailureReason == RegisterFailureReason.INVALID_PASSWORD)
                return StatusCode(400, new Error("Invalid password"));

            if (result.FailureReason == RegisterFailureReason.INVALID_EMAIL)
                return StatusCode(400, new Error("Invalid email"));

            return StatusCode(500);
        }
    }
}

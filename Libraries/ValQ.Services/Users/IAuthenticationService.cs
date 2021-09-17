using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string userName, string password);
    }
}

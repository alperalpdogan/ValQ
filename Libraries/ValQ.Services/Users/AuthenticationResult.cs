using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string token)
        {
            Token = token;
        }

        public AuthenticationResult(AuthenticationFailureReason failureReason)
        {
            FailureReason = failureReason;
        }

        public AuthenticationResult()
        {

        }

        public AuthenticationFailureReason? FailureReason { get; private set; }

        public bool Succesful => !FailureReason.HasValue;

        public string Token { get; private set; }

    }
}

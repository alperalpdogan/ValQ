using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public class RegisterResult
    {
        public RegisterResult()
        {

        }

        public RegisterResult(string token)
        {
            Token = token;
        }

        public RegisterResult(RegisterFailureReason failureReason)
        {
            FailureReason = failureReason;
        }

        public bool Successful => FailureReason == null;

        public RegisterFailureReason? FailureReason { get; private set; }

        public string Token { get; private set; }
    }
}

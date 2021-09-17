using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public enum AuthenticationFailureReason
    {
        USER_NOT_EXISTS = 1,
        WRONG_PASSWORD = 2,
        LOCKED_OUT = 3,
        DELETED = 4
    }
}

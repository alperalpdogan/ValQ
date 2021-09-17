using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public enum RegisterFailureReason
    {
        USER_EXISTS = 10,
        INVALID_PASSWORD = 20,
        INVALID_EMAIL = 30
    }
}

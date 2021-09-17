using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.Users
{
    public enum CreateUserResult
    {
        SUCCESSFUL = 1,
        INVALID_PASSWORD = 2,
        USER_ALREADY_EXISTS =3
    }
}

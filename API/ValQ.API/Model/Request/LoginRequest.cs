using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Framework.Models;

namespace ValQ.API.Model.Request
{
    public class LoginRequest : BaseRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}

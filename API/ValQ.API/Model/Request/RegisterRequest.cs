using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Framework.Models;

namespace ValQ.API.Model.Request
{
    public class RegisterRequest : BaseRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.DTO
{
    public class RegisterUserDTO : BaseDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }

    }
}

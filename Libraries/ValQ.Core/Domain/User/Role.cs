using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Domain.User
{
    public class Role : IdentityRole
    {
        public Role(string name) : base(name)
        {

        }

    }
}

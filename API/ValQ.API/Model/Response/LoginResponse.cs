using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Response
{
    public class LoginResponse
    {
        public JWT JWT { get; set; }
    }

    public class JWT 
    {
        public string Token { get; set; }
    }

}

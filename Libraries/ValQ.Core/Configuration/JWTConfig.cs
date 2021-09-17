using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Configuration
{
    public class JWTConfig
    {
        public string SignInSecret { get; set; }

        public int TokenLifeTimeInMinutes { get; set; }

        public bool ValidateAudience { get; set; }

        public bool ValidateIssuer { get; set; }
    }
}

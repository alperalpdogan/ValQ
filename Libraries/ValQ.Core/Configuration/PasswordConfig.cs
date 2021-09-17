using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Configuration
{
    public class PasswordConfig
    {
        public bool RequiredDigit { get; set; }

        public bool RequireNonAlphanumeric { get; set; }

        public bool RequireUppercase { get; set; }

        public int RequiredLength { get; set; }

        public bool RequireLowercase { get; set; }

        public int RequiredUniqueChars { get; set; }
    }
}

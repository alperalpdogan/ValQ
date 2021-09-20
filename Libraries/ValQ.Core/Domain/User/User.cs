using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Domain.Users
{
    public class User : IdentityUser
    {
        public string NickName { get; set; }

        public int Elo { get; set; }

        public int TotalNumberOfCorrectAnswers { get; set; }

        public int TotalNumberOfIncorrectAnswers { get; set; }
    }
}

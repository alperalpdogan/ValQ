using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.DTO
{
    public class Option
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public bool IsCorrectAnswer { get; set; }
    }
}

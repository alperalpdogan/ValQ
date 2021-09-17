using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.API.Framework.Models
{
    public class Error
    {
        public Error(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public Error()
        {

        }

        public string ErrorMessage { get; set; }
    }
}

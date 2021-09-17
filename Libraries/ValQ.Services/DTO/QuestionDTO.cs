using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.DTO
{
    public class QuestionDTO
    {
        public int Id => new Random().Next();

        public string Body { get; set; }

        public List<Option> Options { get; set; }

    }
}

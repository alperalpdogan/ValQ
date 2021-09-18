using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.DTO
{
    public class Game
    {
        public List<QuestionDTO> Questions { get; set; }

        public Guid Guid { get; set; }

        public int QuestionTimeLimit { get; set; }
    }
}

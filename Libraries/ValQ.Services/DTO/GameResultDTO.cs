using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Services.DTO
{
    public class GameResultDTO
    {
        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }

        public int EloChange { get; set; }

        public int OldElo { get; set; }

        public int NewElo { get; set; }
    }
}

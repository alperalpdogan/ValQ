using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Response
{
    public class GameResult
    {
        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }

        public int EloChange { get; set; }

        public int OldElo { get; set; }

        public int NewElo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Response
{
    public class MatchHistoryLine
    {
        public DateTime PlayedAt { get; set; }

        public int EloChange { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }

        public double CorrectAnswerRate => ((double)NumberOfCorrectAnswers / (double)(NumberOfCorrectAnswers + NumberOfIncorrectAnswers)) * 100;
    }
}

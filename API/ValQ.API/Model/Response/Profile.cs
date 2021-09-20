using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Response
{
    public class Profile
    {
        public string Name { get; set; }

        public Rank Rank { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }

        public int CorrectAnswerRate { get; set; }

        public int Elo{ get; set; }
    }
}

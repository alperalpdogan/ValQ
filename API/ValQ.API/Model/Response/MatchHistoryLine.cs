﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Response
{
    public class MatchHistoryLine
    {
        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }

        public int CorrectAnswerRate => (NumberOfCorrectAnswers / (NumberOfCorrectAnswers + NumberOfIncorrectAnswers)) * 100;
    }
}
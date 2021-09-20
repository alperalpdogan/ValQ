using System;

namespace ValQ.API.Model.Request
{
    public class FinishGameRequest
    {
        public Guid GameId { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public int NumberOfIncorrectAnswers { get; set; }
    }
}

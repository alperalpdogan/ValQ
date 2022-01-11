using System;

namespace ValQ.API.Model.Response
{
    public class RankHistory
    {
        public DateTime ChangedAt { get; set; }

        public Rank NewRank { get; set; }

        public Rank OldRank { get; set; }
    }
}

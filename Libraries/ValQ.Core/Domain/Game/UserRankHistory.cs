using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;
using ValQ.Core.Domain.Users;

namespace ValQ.Core.Domain.Game
{
    public class UserRankHistory : BaseEntity
    {
        public int OldRankId { get; set; }

        public virtual Rank OldRank { get; set; }

        public int NewRankId { get; set; }

        public virtual Rank NewRank { get; set; }

        public DateTime ChangedAt { get; set; }

        public virtual User User { get; set; }

        public string UserId { get; set; }
    }
}

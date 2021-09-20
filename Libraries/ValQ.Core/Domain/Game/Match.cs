using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;
using ValQ.Core.Domain.Users;

namespace ValQ.Core.Domain.Game
{
    public class Match : BaseEntity
    {
        public virtual User User { get; set; }

        public string UserId { get; set; }

        public DateTime PlayedAt { get; set; }

        public int EloChange { get; set; }
    }
}

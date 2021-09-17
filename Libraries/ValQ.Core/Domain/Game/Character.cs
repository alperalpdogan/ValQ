using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class Character : BaseEntity
    {
        public string Name { get; set; }

        public Class Class { get; set; }

        public virtual Origin Origin { get; set; }

        public int OriginId { get; set; }

        public DateTime ReleaseDate { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

    }
}

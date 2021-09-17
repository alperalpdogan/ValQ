using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class Skill : BaseEntity, ILocalizedEntity
    {
        public string Name { get; set; }

        public decimal Cooldown { get; set; }

        public int UsesPerRound { get; set; }

        public int Cost { get; set; }

        public SkillType Type { get; set; }

        public virtual Character Character { get; set; }

        public int CharacterId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class WeaponDamage : BaseEntity
    {
        public virtual Weapon Weapon { get; set; }

        public int WeaponId { get; set; }

        public int DamageFromMaxDistance { get; set; }

        public int DamageFromMinDistance { get; set; }

        public BodyPart DamageToBodyPart { get; set; }
    }
}

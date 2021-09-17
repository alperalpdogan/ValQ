using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class Weapon : BaseEntity
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public int MagazineCapacity { get; set; }

        public WeaponType WeaponType { get; set; }

        public virtual ICollection<WeaponDamage> WeaponDamages { get; set; }

    }
}

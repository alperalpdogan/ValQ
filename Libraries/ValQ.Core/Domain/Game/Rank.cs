using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class Rank : BaseEntity, ILocalizedEntity
    {
        public int EloLowerThreshold { get; set; }

        public int EloUpperThreshold { get; set; }

        public string Name { get; set; }

        public string PictureURL { get; set; }
    }
}

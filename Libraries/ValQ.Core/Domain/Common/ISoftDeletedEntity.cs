using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Domain.Common
{
    public interface ISoftDeletedEntity
    {
        public bool Deleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }
    }
}

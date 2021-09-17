using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Common;

namespace ValQ.Core.Domain.Game
{
    public class QuestionTemplate : BaseEntity, ILocalizedEntity
    {
        public string Template { get; set; }

        public QuestionType Type { get; set; }
    }
}

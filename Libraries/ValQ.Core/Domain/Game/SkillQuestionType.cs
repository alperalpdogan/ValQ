using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Domain.Game
{
    public enum SkillQuestionType
    {
        ISNT_SAME_TYPE = 10,
        COST = 20,
        CHARACTER_NAME = 40,
        FIND_BY_PICTURE = 50,
        DOESNT_BELONG_TO_SAME_CHARACTER = 60,
        SAME_TYPE_WITH_PRESELECTED_SKILL = 70,
        SAME_COST_WITH_PRESELECTED_SKILL = 90
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Configuration
{
    public class GameConfig
    {
        public int TimeLimitForQuestion { get; set; } = 30;

        public int NumberOfQuestionsPerGame { get; set; } = 10;
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Games
{
    public interface IRankService
    {
        Task<Rank> GetRankForElo(int elo);
    }
}
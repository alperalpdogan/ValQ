using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
using ValQ.Data.Repository;

namespace ValQ.Services.Games
{
    public class RankService : IRankService
    {
        private readonly IRepository<Rank> _rankRepository;

        public RankService(IRepository<Rank> rankRepository)
        {
            _rankRepository = rankRepository;
        }

        public async Task<Rank> GetRankForElo(int elo)
        {
            return await _rankRepository.Table
                .Where(o => o.EloLowerThreshold >= elo && o.EloUpperThreshold < elo)
                .SingleOrDefaultAsync();
        }
    }
}

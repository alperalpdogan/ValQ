using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Game;
using ValQ.Data.Extensions;
using ValQ.Data.Repository;

namespace ValQ.Services.Games
{
    public class RankService : IRankService
    {
        private readonly IRepository<Rank> _rankRepository;
        private readonly IRepository<UserRankHistory> _userRankHistoryRepository;

        public RankService(IRepository<Rank> rankRepository,
                           IRepository<UserRankHistory> userRankHistoryRepository)
        {
            _rankRepository = rankRepository;
            _userRankHistoryRepository = userRankHistoryRepository;
        }

        public async Task<Rank> GetRankByIdAsync(int id)
        {
            return await _rankRepository.GetByIdAsync(id);
        }

        public async Task<Rank> GetRankForElo(int elo)
        {
            return await _rankRepository.Table
                .Where(o => elo >= o.EloLowerThreshold)
                .Where(o => elo < o.EloUpperThreshold)
                .SingleAsync();
        }

        public async Task<IPagedList<UserRankHistory>> GetRankHistoryAsync(int pageNumber = 1, int pageSize = int.MaxValue)
        {
            return await _userRankHistoryRepository
                .Table
                .OrderByDescending(o => o.ChangedAt)
                .ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task InsertRankHistoryAsync(UserRankHistory rankHistory)
        {
            if(rankHistory == null)
                throw new ArgumentNullException(nameof(rankHistory));

            await _userRankHistoryRepository.InsertAsync(rankHistory);
        }
    }
}

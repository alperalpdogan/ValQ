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
    public class MatchService : IMatchService
    {
        private readonly IRepository<Match> _matchRepository;

        public MatchService(IRepository<Match> matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<IPagedList<Match>> GetMatchHistoryForUser(string userId, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _matchRepository.Table
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.PlayedAt)
                .ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task InsertMatchAsync(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            await _matchRepository.InsertAsync(match);
        }
    }
}

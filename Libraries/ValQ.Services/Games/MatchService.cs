using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
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

        public async Task InsertMatchAsync(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            await _matchRepository.InsertAsync(match);
        }
    }
}

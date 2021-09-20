using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Games
{
    public interface IMatchService
    {
        Task InsertMatchAsync(Match match);

        Task<IPagedList<Match>> GetMatchHistoryForUser(string userId, int pageNumber = 1, int pageSize = int.MaxValue);
    }
}

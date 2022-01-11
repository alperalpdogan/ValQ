using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Games
{
    public interface IRankService
    {
        Task<Rank> GetRankByIdAsync(int id);

        Task<Rank> GetRankForElo(int elo);

        Task InsertRankHistoryAsync(UserRankHistory rankHistory);

        Task<IPagedList<UserRankHistory>> GetRankHistoryAsync(int pageNumber = 1, int pageSize = int.MaxValue);
    }
}

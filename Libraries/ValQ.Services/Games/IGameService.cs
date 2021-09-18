using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;
using ValQ.Services.DTO;

namespace ValQ.Services.Games
{
    public interface IGameService
    {
        Task<Game> StartNewGameAsync();
    }
}

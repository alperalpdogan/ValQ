using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Common
{
    public interface ICharacterService
    {
        Task<Character> GetCharacterByIdAsync(int characterId);

        Task<Character> GetRandomCharacterAsync();
    }
}

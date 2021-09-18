using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Caching;
using ValQ.Core.Domain.Game;
using ValQ.Data.Repository;

namespace ValQ.Services.Common
{
    public class CharacterService : ICharacterService
    {
        private readonly IRepository<Character> _characterRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public CharacterService(IStaticCacheManager staticCacheManager,
                                IRepository<Character> characterRepository)
        {
            _staticCacheManager = staticCacheManager;
            _characterRepository = characterRepository;
        }

        public async Task<Character> GetCharacterByIdAsync(int characterId)
        {
            if (characterId == 0)
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Character>.ByIdCacheKey, characterId);

            return await _characterRepository.GetByIdAsync(characterId, cache => cacheKey);
        }

        public async Task<Character> GetRandomCharacterAsync()
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _characterRepository.Table.Count());
            return await _characterRepository.Table.Skip(toSkip).Take(1).Include(o => o.Skills).FirstAsync();
        }

    }
}

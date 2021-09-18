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
    public class SkillService : ISkillService
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public SkillService(IRepository<Skill> skillRepository,
                            IStaticCacheManager staticCacheManager)
        {
            _skillRepository = skillRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<Skill> GetRandomSkillExceptCharacterAsync(int characterId)
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _skillRepository.Table.Where(o => o.CharacterId != characterId).Count());

            return await _skillRepository.Table.Where(o => o.Id != characterId).Skip(toSkip).Take(1).FirstAsync();
        }

        public async Task<Skill> GetSkillByIdAsync(int id)
        {
            if (id == 0)
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Skill>.ByIdCacheKey, id);

            return await _skillRepository.GetByIdAsync(id, cache => cacheKey);
        }

        public async Task<IList<Skill>> GetSkillsByIdsAsync(List<int> ids)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Skill>.ByIdsCacheKey, ids);

            return await _skillRepository.GetByIdsAsync(ids, cache => cacheKey);
        }

        public async Task<IList<Skill>> GetSkillsForCharacterAsync(int characterId, SkillType? desiredSkillType = null)
        {
            var entities = _skillRepository.Table;

            if (desiredSkillType.HasValue)
                entities = entities.Where(o => o.Type == desiredSkillType);

            return await entities.Where(o => o.CharacterId == characterId).ToListAsync();


        }
    }
}

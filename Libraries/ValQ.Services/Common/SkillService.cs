using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
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

        public async Task<Skill> GetRandomSkillAsync(int? costGreaterThan = null, SkillType? desiredSkillType = null, SkillType? excludedSkillType = null)
        {
            Random rand = new Random();
            var entities = _skillRepository.Table;

            if (costGreaterThan.HasValue)
                entities = entities.Where(o => o.Cost > costGreaterThan.Value);

            if (desiredSkillType.HasValue)
                entities = entities.Where(o => o.Type == desiredSkillType.Value);

            if (excludedSkillType.HasValue)
                entities = entities.Where(o => o.Type != excludedSkillType.Value);

            int toSkip = rand.Next(1, entities.Count());

            return await _skillRepository.Table.Skip(toSkip).Take(1).FirstAsync();
        }

        public async Task<List<Skill>> GetRandomSkillCost(int desiredNumberOfCost, int? excludedCost = null)
        {
            var entities = _skillRepository.Table
                            .Distinct()
                            .Take(desiredNumberOfCost);

            if (excludedCost.HasValue)
                entities = entities.Where(o => o.Cost != excludedCost);

            return await entities.ToListAsync();
        }

        public async Task<Skill> GetRandomSkillExceptCharacterAsync(int characterId)
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _skillRepository.Table.Where(o => o.CharacterId != characterId).Count());

            return await _skillRepository.Table.Where(o => o.Id != characterId).Skip(toSkip).Take(1).FirstAsync();
        }

        public async Task<Skill> GetRandomSkillExceptForSkills(List<int> excludedSkillIds)
        {
            var entites = _skillRepository.Table;

            if (excludedSkillIds.Count > 0)
                entites = entites.Where(o => !excludedSkillIds.Contains(o.Id));

            var count = entites.Count();
            var random = new Random().Next(0, count - 1);
            return await entites.Skip(random).FirstAsync();
        }

        public async Task<List<Skill>> GetRandomSkillsByCost(int desiredCost, int desiredNumber)
        {
            var entities = await _skillRepository.Table
                            .Distinct()
                            .Take(desiredNumber)
                            .ToListAsync();

            if (entities.Count < 4)
                throw new ValQException($"Could not find desired number: {desiredNumber} of skills with cost: {desiredCost}");

            return entities;
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

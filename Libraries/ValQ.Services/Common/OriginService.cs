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
    public class OriginService : IOriginService
    {
        private readonly IRepository<Origin> _originRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public OriginService(IRepository<Origin> originRepository,
                             IStaticCacheManager staticCacheManager)
        {
            _originRepository = originRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<List<Origin>> GetAllOriginsAsync()
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Origin>.AllCacheKey);

            return (await _originRepository.GetAllAsync(query => query, cache => cacheKey)).ToList();
        }

        public async Task<Origin> GetOriginByIdAsync(int id)
        {
            if (id == 0)
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Origin>.ByIdCacheKey, id);
            return await _originRepository.GetByIdAsync(id, cache => cacheKey);
        }
    }
}

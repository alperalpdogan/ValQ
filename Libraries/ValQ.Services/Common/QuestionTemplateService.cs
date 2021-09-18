using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Caching;
using ValQ.Core.Domain.Game;
using ValQ.Data.Repository;
using ValQ.Services.Common.Caching;

namespace ValQ.Services.Common
{
    public class QuestionTemplateService : IQuestionTemplateService
    {
        private readonly IRepository<QuestionTemplate> _questionTemplateRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public QuestionTemplateService(IRepository<QuestionTemplate> questionTemplateRepository,
                                       IStaticCacheManager staticCacheManager)
        {
            _questionTemplateRepository = questionTemplateRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<QuestionTemplate> GetQuestionTemplateByIdAsync(int templateId)
        {
            if (templateId == 0)
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<QuestionTemplate>.ByIdCacheKey, templateId);

            return await _questionTemplateRepository.GetByIdAsync(templateId, cache => cacheKey);
        }

        public async Task<QuestionTemplate> GetQuestionTemplateByTypeAndDescriptorAsync(QuestionType type, int typeDescriptor) 
        { 
        
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(QuestionTemplateCachingDefaults.QuestionTemplateByTypeAndDescriptorCacheKey, type, typeDescriptor);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                return await _questionTemplateRepository.Table
                .Where(o => o.Type == type && o.TypeDescriptor == typeDescriptor)
                .SingleOrDefaultAsync();
            });
        }
    }
}

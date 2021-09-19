using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Caching;

namespace ValQ.Services.Common.Caching
{
    public class QuestionTemplateCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Question Type
        /// {1} : Question Type Descriptor
        /// </remarks>
        public static CacheKey QuestionTemplateByTypeAndDescriptorCacheKey => new CacheKey("ValQ.questionTemplate.byTypeAndDescriptor.{0}-{1}");

    }
}

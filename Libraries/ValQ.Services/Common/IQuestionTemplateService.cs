using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Common
{
    public interface IQuestionTemplateService
    {
        Task<QuestionTemplate> GetQuestionTemplateByIdAsync(int templateId);

        Task<QuestionTemplate> GetQuestionTemplateByTypeAndDescriptorAsync(QuestionType type, int typeDescriptor);
    }
}

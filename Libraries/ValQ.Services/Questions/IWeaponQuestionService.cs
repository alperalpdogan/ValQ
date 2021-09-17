using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;
using ValQ.Services.DTO;

namespace ValQ.Services.Questions
{
    public interface IWeaponQuestionService
    {
        Task<QuestionDTO> GenerateWeaponMaxDistanceDamageForRandomBodyPartQuestionAsync();

        Task<QuestionDTO> GenerateWeaponMinDistanceDamageForRandomBodyPartQuestionAsync();

        Task<QuestionDTO> GenerateWeaponCostQuestionAsync();
    }
}

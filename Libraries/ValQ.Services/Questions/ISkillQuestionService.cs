using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;
using ValQ.Services.DTO;

namespace ValQ.Services.QuestionDTOs
{
    public interface ISkillQuestionService
    {
        Task<QuestionDTO> GenerateSkillIsntSameTypeQuestionAsync();

        Task<QuestionDTO> GenerateSkillIsSameTypeQuestionAsync();

        Task<QuestionDTO> GenerateSkillCostQuestionAsync();

        Task<QuestionDTO> GenerateSkillFindByPictureQuesitonAsync();

        Task<QuestionDTO> GenerateSkillDoesntBelongToSameCharacterQuestionAsync();

        Task<QuestionDTO> GenerateSkillSameCostWithPreselectedQuestionAsync();

        Task<QuestionDTO> GenerateSkillBelongsToCharacterQuestionAsync();
    }
}

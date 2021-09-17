using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;
using ValQ.Services.DTO;

namespace ValQ.Services.Questions
{
    public interface ICharacterQuestionService
    {

        Task<QuestionDTO> GenerateCharacterClassQuestionAsync();

        Task<QuestionDTO> GenerateCharacterUltimateSkillNameAsync();

        Task<QuestionDTO> GenerateCharacterRequiredPointsForUltimateAsync();

        Task<QuestionDTO> GenerateCharacterReleaseDateQuestionAsync();

        Task<QuestionDTO> GenerateCharacterSkillDoentBelongQuestionAsync();

        Task<QuestionDTO> GenerateCharacterOriginQuestionAsync();

        Task<QuestionDTO> GenerateCharacterSignatureSkillNameQuestionAsync();

    }
}

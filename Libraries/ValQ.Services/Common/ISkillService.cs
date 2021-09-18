using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Common
{
    public interface ISkillService
    {
        Task<Skill> GetSkillByIdAsync(int id);

        Task<IList<Skill>> GetSkillsByIdsAsync(List<int> ids);

        Task<IList<Skill>> GetSkillsForCharacterAsync(int characterId, SkillType? desiredSkillType = null);

        Task<Skill> GetRandomSkillExceptCharacterAsync(int characterId);
    }
}

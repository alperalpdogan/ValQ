using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Domain.User;
using ValQ.Core.Util;
using ValQ.Core;
using ValQ.Data.Repository;
using ValQ.Services.DTO;
using ValQ.Services.Questions;
using ValQ.Core.Infrastructure;
using ValQ.Core.Configuration;

namespace ValQ.Services.Games
{
    public class GameService : IGameService
    {
        #region Fields
        private readonly ICharacterQuestionService _characterQuestionService;
        private readonly ISkillQuestionService _skillQuestionService;
        private readonly IWeaponQuestionService _weaponQuestionService;
        private readonly AppSettings _appSettings;
        #endregion

        #region Ctor
        public GameService(ICharacterQuestionService characterQuestionService,
                           ISkillQuestionService skillQuestionService,
                           IWeaponQuestionService weaponQuestionService)
        {
            _characterQuestionService = characterQuestionService;
            _skillQuestionService = skillQuestionService;
            _weaponQuestionService = weaponQuestionService;
            _appSettings = Singleton<AppSettings>.Instance;
        }
        #endregion

        #region Methods
        public async Task<Game> StartNewGameAsync()
        {
            Game game = new Game();
            game.Guid = Guid.NewGuid();
            game.QuestionTimeLimit = _appSettings.GameConfig.TimeLimitForQuestion;
            List<QuestionDTO> questions = new List<QuestionDTO>();

            List<QuestionAndTypes> questionTypes = new List<QuestionAndTypes>();


            var characterQuestions = EnumHelper<CharacterQuestionType>.ConvertToList();

            //add character based questions to the dictionary
            foreach (var characterQuestionType in characterQuestions)
            {
                questionTypes.Add(new QuestionAndTypes(QuestionType.CHARACTER, (int)characterQuestionType));
            }

            //add skill based questions to the dictionary
            var skillQuestions = EnumHelper<SkillQuestionType>.ConvertToList();
            foreach (var skillQuestionType in skillQuestions)
            {
                questionTypes.Add(new QuestionAndTypes(QuestionType.SKILL, (int)skillQuestionType));
            }

            //add weapong based questions to the dictionary
            var weaponQuestions = EnumHelper<WeaponQuestionType>.ConvertToList();
            foreach (var weaponQuestionType in weaponQuestions)
            {
                questionTypes.Add(new QuestionAndTypes(QuestionType.WEAPON, (int)weaponQuestionType));
            }

            if (questionTypes.Count > _appSettings.GameConfig.NumberOfQuestionsPerGame)
            {
                //remove items from the list until number of question is reached
            }

            foreach (var item in questionTypes)
            {
                switch (item.Type)
                {
                    case QuestionType.CHARACTER:
                        if (item.TypeDescriptor == (int)CharacterQuestionType.CLASS)
                            questions.Add(await _characterQuestionService.GenerateCharacterClassQuestionAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.ORIGIN)
                            questions.Add(await _characterQuestionService.GenerateCharacterOriginQuestionAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.RELEASE_DATE)
                            questions.Add(await _characterQuestionService.GenerateCharacterReleaseDateQuestionAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.REQUIRED_POINTS_FOR_ULTIMATE)
                            questions.Add(await _characterQuestionService.GenerateCharacterRequiredPointsForUltimateAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.SIGNATURE_SKILL_NAME)
                            questions.Add(await _characterQuestionService.GenerateCharacterSignatureSkillNameQuestionAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.SKILL_DOESNT_BELONG)
                            questions.Add(await _characterQuestionService.GenerateCharacterSkillDoentBelongQuestionAsync());
                        else if (item.TypeDescriptor == (int)CharacterQuestionType.ULTIMATE_SKILL_NAME)
                            questions.Add(await _characterQuestionService.GenerateCharacterUltimateSkillNameAsync());
                        break;

                    case QuestionType.SKILL:
                        if (item.TypeDescriptor == (int)SkillQuestionType.CHARACTER_NAME)
                            questions.Add(await _skillQuestionService.GenerateSkillBelongsToCharacterQuestionAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.COST)
                            questions.Add(await _skillQuestionService.GenerateSkillCostQuestionAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.DOESNT_BELONG_TO_SAME_CHARACTER)
                            questions.Add(await _skillQuestionService.GenerateSkillDoesntBelongToSameCharacterQuestionAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.FIND_BY_PICTURE)
                            questions.Add(await _skillQuestionService.GenerateSkillFindByPictureQuesitonAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.ISNT_SAME_TYPE)
                            questions.Add(await _skillQuestionService.GenerateSkillIsntSameTypeQuestionAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.SAME_COST_WITH_PRESELECTED_SKILL)
                            questions.Add(await _skillQuestionService.GenerateSkillSameCostWithPreselectedQuestionAsync());
                        else if (item.TypeDescriptor == (int)SkillQuestionType.SAME_TYPE_WITH_PRESELECTED_SKILL)
                            questions.Add(await _skillQuestionService.GenerateSkillIsSameTypeQuestionAsync());
                        break;

                    case QuestionType.WEAPON:
                        if (item.TypeDescriptor == (int)WeaponQuestionType.COST_FOR_THE_WEAPON)
                            questions.Add(await _weaponQuestionService.GenerateWeaponCostQuestionAsync());
                        else if (item.TypeDescriptor == (int)WeaponQuestionType.MAX_DISTANCE_DAMAGE_FOR_RANDOM_BODY_PART)
                            questions.Add(await _weaponQuestionService.GenerateWeaponMaxDistanceDamageForRandomBodyPartQuestionAsync());
                        else if (item.TypeDescriptor == (int)WeaponQuestionType.MIN_DISTANCE_DAMAGE_FOR_RANDOM_BODY_PART)
                            questions.Add(await _weaponQuestionService.GenerateWeaponMinDistanceDamageForRandomBodyPartQuestionAsync());
                        break;
                }

            }

            questions.Shuffle();
            game.Questions = questions;
            return game;
        }
        #endregion

        #region Nested Class
        private class QuestionAndTypes
        {
            public QuestionAndTypes(QuestionType questionType, int typeDescriptor)
            {
                Type = questionType;
                TypeDescriptor = typeDescriptor;
            }

            public QuestionType Type { get; set; }

            public int TypeDescriptor { get; set; }
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Game;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Util;
using ValQ.Data.Repository;
using ValQ.Services.Common;
using ValQ.Services.DTO;
using ValQ.Services.Localization;

namespace ValQ.Services.Questions
{
    public class SkillQuestionService : ISkillQuestionService
    {
        #region Fields
        private readonly ISkillService _skillService;
        private readonly IQuestionTemplateService _questionTemplateService;
        private readonly ICharacterService _characterService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<Character> _characterRepository;
        #endregion

        #region Ctor
        public SkillQuestionService(ISkillService skillService,
                                    IQuestionTemplateService questionTemplateService,
                                    ICharacterService characterService,
                                    ILocalizationService localizationService,
                                    IRepository<Skill> skillRepository,
                                    IRepository<Character> characterRepository)
        {
            _skillService = skillService;
            _questionTemplateService = questionTemplateService;
            _characterService = characterService;
            _localizationService = localizationService;
            _skillRepository = skillRepository;
            _characterRepository = characterRepository;
        }

        #endregion

        #region Private Methods
        private async Task<QuestionTemplate> GetQuestionTemplate(SkillQuestionType questionType)
        {
            return await _questionTemplateService.GetQuestionTemplateByTypeAndDescriptorAsync(QuestionType.SKILL, (int)questionType);
        }
        #endregion

        #region Methods


        public async Task<QuestionDTO> GenerateSkillCostQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.COST);
            var randomSkillWithCost = await _skillService.GetRandomSkillAsync(0);
            var correctAnswer = randomSkillWithCost.Cost;

            List<int> costs = new List<int>();
            costs.Add(correctAnswer);
            var randCosts = (await _skillService.GetRandomSkillCost(3, correctAnswer)).Select(o => o.Cost);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = correctAnswer.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var cost in randCosts)
            {
                options.Add(new Option()
                {
                    Body = cost.ToString(),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedSkillName = await _localizationService.GetLocalizedAsync(randomSkillWithCost, s => s.Name);
            var localizedTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);

            question.Body = localizedTemplate.Replace("{SKILL_NAME}", localizedSkillName);

            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateSkillDoesntBelongToSameCharacterQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.DOESNT_BELONG_TO_SAME_CHARACTER);

            var randChar = await _characterService.GetRandomCharacterAsync();

            var skillsForCharacter = await _skillService.GetSkillsForCharacterAsync(randChar.Id);
            var randSkillDoesntBelongToCharacter = await _skillService.GetRandomSkillExceptCharacterAsync(randChar.Id);
            var skillName = await _localizationService.GetLocalizedAsync(randSkillDoesntBelongToCharacter, s => s.Name);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = skillName,
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var item in skillsForCharacter.Take(3))
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(item, s => s.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedTemplate.Replace("{CHARACTER_NAME}", randChar.Name);



            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateSkillFindByPictureQuesitonAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.FIND_BY_PICTURE);
            Skill randSkill = await _skillService.GetRandomSkillAsync();
            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(randSkill, s => s.Name),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            }) ;

            List<Skill> skills = new List<Skill>();

            for (int i = 0; i < 3; i++)
            {
                var count = _skillRepository.Table.Where(o => o != randSkill && !skills.Contains(o)).Count();

                var randSkip = new Random().Next(0, count);

                skills.Add(_skillRepository.Table.Where(o => o != randSkill && !skills.Contains(o)).Skip(randSkip).Take(1).First());
            }

            foreach (var item in skills)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(item, s => s.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            question.Body = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateSkillIsntSameTypeQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.ISNT_SAME_TYPE);

            var skillList = EnumHelper<SkillType>.ConvertToList();


            var randomSkill = await _skillService.GetRandomSkillAsync();
            var randomSkillType = randomSkill.Type;
            var threeSkillsFromSameType = new List<Skill>();

            for (int i = 0; i < 3; i++)
            {
                if (threeSkillsFromSameType.Count() != 0)
                    threeSkillsFromSameType.Add(_skillRepository.Table.Where(o => o.Type == randomSkillType).Where(o => !threeSkillsFromSameType.Contains(o)).Take(1).First());
                else
                    threeSkillsFromSameType.Add(_skillRepository.Table.Where(o => o.Type == randomSkillType).Take(1).First());

            }

            var randDifferentTypeSkill = _skillRepository.Table
                .Where(o => o.Type != randomSkillType)
                .Skip(new Random().Next(0, _skillRepository.Table.Where(o => o.Type != randomSkillType).Count()))
                .Take(1).First();

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(randDifferentTypeSkill, s => s.Name),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });


            foreach (var item in threeSkillsFromSameType)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(item, s => s.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, t => t.Template);
            var skillLocalizedName = await _localizationService.GetLocalizedAsync(randomSkill, s => s.Name);
            question.Body = localizedQuestionTemplate.Replace("{SKILL_NAME}", skillLocalizedName);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateSkillIsSameTypeQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.SAME_TYPE_WITH_PRESELECTED_SKILL);

            var randSkill = await _skillService.GetRandomSkillAsync();

            var randSkillWithTheSameType = await _skillService.GetRandomSkillAsync(desiredSkillType: randSkill.Type);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(randSkillWithTheSameType, s => s.Name),
                Id = new Random().Next(),
                IsCorrectAnswer = true
            });


            var randomlySelectedDiffTypeSkills = new List<Skill>();

            //generate 3 wrong option
            while(randomlySelectedDiffTypeSkills.Count != 3)
            {
                var randSkillWithDifferentType = await _skillService.GetRandomSkillAsync(excludedSkillType: randSkill.Type);

                //if duplicate skill is selected continue
                if (randomlySelectedDiffTypeSkills.Contains(randSkillWithDifferentType))
                    continue;

                randomlySelectedDiffTypeSkills.Add(randSkillWithDifferentType);

                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(randSkillWithDifferentType, s => s.Name),
                    Id = new Random().Next(),
                    IsCorrectAnswer = false
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            var randSkillLocalizedName = await _localizationService.GetLocalizedAsync(randSkill, s => s.Name);
            question.Body = localizedQuestionTemplate.Replace("{SKILL_NAME}", randSkillLocalizedName);
            options.Shuffle();
            question.Options = options;

            return question;
        }


        public async Task<QuestionDTO> GenerateSkillSameCostWithPreselectedQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.SAME_COST_WITH_PRESELECTED_SKILL);
            var preSelectedRandSkillWithCost = await _skillService.GetRandomSkillAsync(0);

            List<Skill> skills = new List<Skill>();
            //fetch 3 skills that doesn't have the same cost
            for (int i = 0; i < 3; i++)
            {
                var totalCount = _skillRepository.Table.Where(o => o.Cost != preSelectedRandSkillWithCost.Cost && !skills.Contains(o)).Count();
                var randSkip = new Random().Next(0, totalCount);
                skills.Add(_skillRepository.Table.Where(o => o.Cost != preSelectedRandSkillWithCost.Cost && !skills.Contains(o)).Skip(randSkip).Take(1).First());
            }

            //fetch 1 skill that has the same cost as the preselected skill
            var count = _skillRepository.Table.Where(o => o.Cost == preSelectedRandSkillWithCost.Cost && o != preSelectedRandSkillWithCost).Count();
            var skip = new Random().Next(0, count);
            var correctSkill = _skillRepository.Table.Where(o => o.Cost == preSelectedRandSkillWithCost.Cost && o != preSelectedRandSkillWithCost).Skip(skip).Take(1).First();

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                IsCorrectAnswer = true,
                Body = await _localizationService.GetLocalizedAsync(correctSkill, s => s.Name),
                Id = new Random().Next()
            });

            foreach (var item in skills)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(item, s => s.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, s => s.Template);
            var localizedSkillName = await _localizationService.GetLocalizedAsync(preSelectedRandSkillWithCost, s => s.Name);
            question.Body = localizedQuestionTemplate.Replace("{SKILL_NAME}", localizedSkillName);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateSkillBelongsToCharacterQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = await GetQuestionTemplate(SkillQuestionType.CHARACTER_NAME);

            var randSkill = await _skillService.GetRandomSkillAsync();
            var correctAnswer = (await _characterService.GetCharacterByIdAsync(randSkill.CharacterId)).Name;

            List<string> charNames = new List<string>();
            charNames.Add(correctAnswer);

            for (int i = 0; i < 3; i++)
            {
                int skip = new Random().Next(1, _characterRepository.Table.Where(o => !charNames.Contains(o.Name)).Count());
                var newCharacter = _characterRepository.Table.Where(o => !charNames.Contains(o.Name)).Skip(skip).Take(1).Select(o => o.Name).First();

                charNames.Add(newCharacter);
            }

            List<Option> options = new List<Option>();
            options.Add(new Option()
            {
                Body = correctAnswer,
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var name in charNames.Where(o => o != correctAnswer))
            {
                options.Add(new Option()
                {
                    Body = name,
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            options.Shuffle();

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            var localizedSkillName = await _localizationService.GetLocalizedAsync(randSkill, s => s.Name);
            question.Body = localizedQuestionTemplate.Replace("{SKILL_NAME}", localizedSkillName);
            question.Options = options;

            return question;
        }

        #endregion
    }
}

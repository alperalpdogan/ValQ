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
using ValQ.Services.DTO;
using ValQ.Services.Localization;

namespace ValQ.Services.Questions
{
    public class SkillQuestionService : ISkillQuestionService
    {
        #region Fields
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<QuestionTemplate> _templateRepository;
        private readonly IRepository<Character> _characterRepository;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public SkillQuestionService(IRepository<Skill> skillRepository,
                                    IRepository<QuestionTemplate> templateRepository,
                                    IRepository<Character> characterRepository,
                                    ILocalizationService localizationService)
        {
            _skillRepository = skillRepository;
            _templateRepository = templateRepository;
            _characterRepository = characterRepository;
            _localizationService = localizationService;
        }
        #endregion

        #region Private Methods
        private List<Skill> GetSkillsForCharacter(long characterId)
        {
            return _skillRepository.Table.Where(o => o.CharacterId == characterId).ToList();
        }

        private Skill GetRandomSkillExceptCharacter(long characterId)
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _skillRepository.Table.Where(o => o.CharacterId != characterId).Count());

            return _skillRepository.Table.Where(o => o.Id != characterId).Skip(toSkip).Take(1).First();
        }

        private Skill GetRandomSkill()
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _skillRepository.Table.Count());

            return _skillRepository.Table.Skip(toSkip).Take(1).Include(o => o.Character).First();
        }

        private QuestionTemplate GetQuestionTemplate(SkillQuestionType questionType)
        {
            return _templateRepository.Table.Where(o => o.TypeDescriptor == (int)questionType && o.Type == QuestionType.SKILL).First();

        }
        #endregion

        #region Methods


        public async Task<QuestionDTO> GenerateSkillCostQuestionAsync()
        {
            var question = new QuestionDTO();
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.COST);
            int skip = new Random().Next(1, _skillRepository.Table.Where(o => o.Cost > 0).Count());
            var randomSkillWithCost = _skillRepository.Table.Where(o => o.Cost > 0).Skip(skip).Take(1).First();
            var correctAnswer = randomSkillWithCost.Cost;

            List<int> costs = new List<int>();
            costs.Add(correctAnswer);
            var randCosts = _skillRepository.Table.Select(s => s.Cost).Distinct().Take(4).ToList();

            if (randCosts.Contains(correctAnswer))
                randCosts.Remove(correctAnswer);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = correctAnswer.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var cost in randCosts.Take(3))
            {
                options.Add(new Option()
                {
                    Body = cost.ToString(),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedBody = await _localizationService.GetLocalizedAsync(questionTemplate, temp => temp.Template);
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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.DOESNT_BELONG_TO_SAME_CHARACTER);

            var randChar = _characterRepository.Table.Skip(new Random().Next(0, _characterRepository.Table.Count())).Take(1).First();

            var skillsForCharacter = GetSkillsForCharacter(randChar.Id);
            var randSkillDoesntBelongToCharacter = GetRandomSkillExceptCharacter(randChar.Id);
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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.FIND_BY_PICTURE);
            Skill randSkill = GetRandomSkill();
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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.ISNT_SAME_TYPE);

            var skillList = EnumHelper<SkillType>.ConvertToList();


            var randomSkill = GetRandomSkill();
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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.SAME_TYPE_WITH_PRESELECTED_SKILL);

            var randSkill = GetRandomSkill();

            var toSkip = new Random().Next(1, _skillRepository.Table.Where(o => o.Type == randSkill.Type && o.Id != randSkill.Id).Count());

            var randSkillWithTheSameType = _skillRepository.Table.Where(o => o.Type == randSkill.Type && o.Id != randSkill.Id).Skip(toSkip).Take(1).First();

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(randSkillWithTheSameType, s => s.Name),
                Id = new Random().Next(),
                IsCorrectAnswer = true
            });


            var randomlySelectedDiffTypeSkills = new List<Skill>();

            //generate 3 wrong option
            for (int i = 0; i < 3; i++)
            {
                toSkip = new Random().Next(1, _skillRepository.Table.Where(o => o.Type != randSkill.Type && o.Id != randSkill.Id).Count() - 1);

                var randSkillWithDifferentType = _skillRepository.Table
                    .Where(o => o.Type != randSkill.Type)
                    .Where(o => !randomlySelectedDiffTypeSkills.Select(o => o.Id).Contains(o.Id))
                    .Skip(toSkip).Take(1).First();

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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.SAME_COST_WITH_PRESELECTED_SKILL);

            var totalPreCount = _skillRepository.Table.Where(o => o.Cost > 0).Count();
            var random = new Random().Next(0, totalPreCount);
            var preSelectedRandSkillWithCost = _skillRepository.Table.Where(o => o.Cost > 0).Skip(random).Take(1).First();

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
            QuestionTemplate questionTemplate = GetQuestionTemplate(SkillQuestionType.CHARACTER_NAME);

            var randSkill = GetRandomSkill();
            var correctAnswer = randSkill.Character.Name;

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

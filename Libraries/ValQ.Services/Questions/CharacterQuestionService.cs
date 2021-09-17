using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Util;
using ValQ.Core;
using ValQ.Data.Repository;
using ValQ.Services.DTO;
using Microsoft.EntityFrameworkCore;
using ValQ.Services.Localization;

namespace ValQ.Services.Questions
{
    public class CharacterQuestionService : ICharacterQuestionService
    {
        #region Fields
        private readonly IRepository<Character> _characterRepository;
        private readonly IRepository<QuestionTemplate> _templateRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Origin> _originRepository;
        #endregion

        #region Ctor
        public CharacterQuestionService(IRepository<Character> characterRepository,
                                        IRepository<QuestionTemplate> questionTemplateRepository,
                                        IRepository<Skill> skillRepository,
                                        ILocalizationService localizationService,
                                        IRepository<Origin> originRepository)
        {
            _characterRepository = characterRepository;
            _templateRepository = questionTemplateRepository;
            _skillRepository = skillRepository;
            _localizationService = localizationService;
            _originRepository = originRepository;
        }
        #endregion

        #region Private methods
        private QuestionTemplate GetQuestionTemplate(CharacterQuestionType questionType)
        {
            return _templateRepository.Table.Where(o => o.TypeDescriptor == (int)questionType && o.Type == QuestionType.CHARACTER).First();

        }

        private Skill GetRandomSkillExceptCharacter(long characterId)
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _skillRepository.Table.Where(o => o.CharacterId != characterId).Count());

            return _skillRepository.Table.Where(o => o.Id != characterId).Skip(toSkip).Take(1).First();
        }


        private Character GetRandomCharacter()
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _characterRepository.Table.Count());
            return _characterRepository.Table.Skip(toSkip).Take(1).Include(o => o.Skills).First();
        }

        #endregion

        #region Methods
        public async Task<QuestionDTO> GenerateCharacterClassQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.CLASS);
            var randCharacter = GetRandomCharacter();
            var correctAnswer = randCharacter.Class;

            //gets all the classes
            var listOfClasses = EnumHelper<Class>.ConvertToList();

            //removes the one that is the correct class
            listOfClasses.Remove(correctAnswer);

            List<Option> options = new List<Option>();

            //adds the correct option to the list
            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedEnumAsync(correctAnswer),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var characterClass in listOfClasses)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedEnumAsync(characterClass),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }


            //shuffle the options

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);
            options.Shuffle();

            question.Options = options;
            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterReleaseDateQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.RELEASE_DATE);
            var randCharacter = GetRandomCharacter();
            string dateTimeFormat = "dd MMMM yyyy";
            var dateTimeCulture = CultureInfo.CreateSpecificCulture("en-EN");

            var correctAnswer = randCharacter.ReleaseDate;

            //number of days that's gonna be lower than the correct answer
            int daysToRemove = new Random().Next(1, 2);

            int daysToAdd = 3 - daysToRemove;

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = correctAnswer.ToString(dateTimeFormat, dateTimeCulture),
                Id = new Random().Next(),
                IsCorrectAnswer = true,
            });

            for (int i = 0; i < daysToRemove; i++)
            {
                int numberOfDaysToSubtract = new Random().Next(1, 14);

                DateTime generatedDate = randCharacter.ReleaseDate - new TimeSpan(numberOfDaysToSubtract, 0, 0, 0, 0);

                options.Add(new Option()
                {
                    Body = generatedDate.ToString(dateTimeFormat, dateTimeCulture),
                    Id = new Random().Next(),
                    IsCorrectAnswer = false
                });
            }

            for (int i = 0; i < daysToAdd; i++)
            {
                int numberOfDaysToAdd = new Random().Next(1, 14);

                DateTime generatedDate = randCharacter.ReleaseDate.AddDays(numberOfDaysToAdd);

                options.Add(new Option()
                {
                    Body = generatedDate.ToString(dateTimeFormat, dateTimeCulture),
                    Id = new Random().Next(),
                    IsCorrectAnswer = false
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterRequiredPointsForUltimateAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.REQUIRED_POINTS_FOR_ULTIMATE);
            var randCharacter = GetRandomCharacter();

            var correctAnswer = randCharacter.RequiredPointsForUltimateSkill;

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = correctAnswer.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            List<int> optionValues = new List<int> { 5, 6, 7, 8 };

            optionValues.Remove(correctAnswer);

            foreach (var value in optionValues)
            {
                options.Add(new Option()
                {
                    Body = value.ToString(),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }


            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);
            question.Options = options;

            options.Shuffle();
            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterSkillDoentBelongQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.SKILL_DOESNT_BELONG);
            var randCharacter = GetRandomCharacter();

            var correctSkill = randCharacter.Skills.First();
            var randSkillsExcept = GetRandomSkillExceptCharacter(randCharacter.Id);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(correctSkill, s => s.Name),
                Id = new Random().Next(),
                IsCorrectAnswer = true
            });

            //get random 3 skill
            for (int i = 0; i < 3; i++)
            {
                var randSkill = GetRandomSkillExceptCharacter(randCharacter.Id);

                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(randSkill, s => s.Name),
                    Id = new Random().Next(),
                    IsCorrectAnswer = true
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            var correctSkillLocalizedName = await _localizationService.GetLocalizedAsync(correctSkill, s => s.Name);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", correctSkillLocalizedName);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterUltimateSkillNameAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.ULTIMATE_SKILL_NAME);
            var randCharacter = GetRandomCharacter();

            var ultimateForCharacter = randCharacter.Skills.Where(o => o.Type == SkillType.Ultimate).First();

            List<Skill> randSkills = new List<Skill>();

            for (int i = 0; i < 3; i++)
            {
                randSkills.Add(GetRandomSkillExceptCharacter(randCharacter.Id));
            }


            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(ultimateForCharacter, s => s.Name),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var skill in randSkills)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(skill, s => s.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }


            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);

            options.Shuffle();
            question.Options = options;
            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterOriginQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.ORIGIN);
            var randCharacter = GetRandomCharacter();

            var correctAnswer = randCharacter.Origin;
            var listOfOrigins = await _originRepository.Table.ToListAsync();

            listOfOrigins.Remove(correctAnswer);

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(correctAnswer, o => o.Name),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });


            //shuffle the list before we add the remaining options
            listOfOrigins.Shuffle();

            for (int i = 0; i < 3; i++)
            {
                var characterClass = listOfOrigins[i];
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(characterClass, o => o.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, t => t.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateCharacterSignatureSkillNameQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(CharacterQuestionType.SIGNATURE_SKILL_NAME);
            var randCharacter = GetRandomCharacter();
            var correctAnswer = randCharacter.Skills.Where(o => o.Type == SkillType.Signature).First();
            int numberOfRandomSkills = new Random().Next(0, 2);
            List<Skill> skills = new List<Skill>();

            for (int i = 0; i < numberOfRandomSkills; i++)
            {
                skills.Add(GetRandomSkillExceptCharacter(randCharacter.Id));
            }

            for (int i = 0; i < 3 - numberOfRandomSkills; i++)
            {
                foreach (var skill in skills)
                    randCharacter.Skills.Remove(skill);

                randCharacter.Skills.ToList().Shuffle();

                skills.Add(randCharacter.Skills.First());
            }

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = await _localizationService.GetLocalizedAsync(correctAnswer, c => c.Name),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            foreach (var skill in skills)
            {
                options.Add(new Option()
                {
                    Body = await _localizationService.GetLocalizedAsync(skill, c => c.Name),
                    IsCorrectAnswer = false,
                    Id = new Random().Next()
                });
            }

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{CHARACTER_NAME}", randCharacter.Name);
            options.Shuffle();
            question.Options = options;

            return question;
        }
        #endregion

    }
}

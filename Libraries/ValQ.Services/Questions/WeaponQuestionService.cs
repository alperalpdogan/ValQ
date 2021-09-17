using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Util;
using ValQ.Core;
using ValQ.Data.Repository;
using ValQ.Services.DTO;
using ValQ.Services.Localization;
using Microsoft.EntityFrameworkCore;

namespace ValQ.Services.Questions
{
    public class WeaponQuestionService : IWeaponQuestionService
    {

        #region Fields
        private readonly IRepository<Weapon> _weaponRepository;
        private readonly IRepository<QuestionTemplate> _templateRepository;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public WeaponQuestionService(IRepository<Weapon> weaponRepository,
                                     IRepository<QuestionTemplate> templateRepository,
                                     ILocalizationService localizationService)
        {
            _weaponRepository = weaponRepository;
            _templateRepository = templateRepository;
            _localizationService = localizationService;
        }
        #endregion

        #region Private Methods
        private QuestionTemplate GetQuestionTemplate(WeaponQuestionType questionType)
        {
            return _templateRepository.Table.Where(o => o.TypeDescriptor == (int)questionType && o.Type == QuestionType.WEAPON).First();
        }


        private Weapon GetRandomWeapon()
        {
            Random rand = new Random();
            int toSkip = rand.Next(1, _weaponRepository.Table.Count());

            return _weaponRepository.Table.Skip(toSkip).Take(1).Include(o => o.WeaponDamages).First();
        }
        #endregion

        #region Methods
        public async Task<QuestionDTO> GenerateWeaponCostQuestionAsync()
        {
            var question = new QuestionDTO();

            var questionTemplate = GetQuestionTemplate(WeaponQuestionType.COST_FOR_THE_WEAPON);

            var randWeapon = GetRandomWeapon();

            //gets another weapon for the same type. we're using it the create one random incorrect option
            int skip = new Random().Next(0, _weaponRepository.Table.Where(o => o.WeaponType == randWeapon.WeaponType && o.Id != randWeapon.Id).Count());
            var sameTypeWeapon = _weaponRepository.Table.Where(o => o.WeaponType == randWeapon.WeaponType && o.Id != randWeapon.Id).Skip(skip).Take(1).First();

            List<Option> options = new List<Option>();

            options.Add(new Option()
            {
                Body = randWeapon.Price.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            options.Add(new Option()
            {
                Body = sameTypeWeapon.Price.ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            //genarete two random incorrect option
            for (int i = 0; i < 2; i++)
            {
                int randomNumberToAddOrSubtract = 0;
                if (randWeapon.Price < 500)
                    randomNumberToAddOrSubtract = new Random().Next(1, 2) * 100;

                else if (randWeapon.Price >= 500 && randWeapon.Price < 1100)
                    randomNumberToAddOrSubtract = new Random().Next(1, 3) * 100;

                else if (randWeapon.Price >= 1100 && randWeapon.Price < 2000)
                    randomNumberToAddOrSubtract = new Random().Next(1, 4) * 100;

                else if (randWeapon.Price >= 2000 && randWeapon.Price < 3000)
                    randomNumberToAddOrSubtract = new Random().Next(1, 5) * 100;

                else
                    randomNumberToAddOrSubtract = new Random().Next(1, 6) * 100;

                //%50 chance to subtract the generated number
                if (new Random().Next(0, 1) == 0)
                    options.Add(new Option()
                    {
                        Body = (randWeapon.Price - randomNumberToAddOrSubtract).ToString(),
                        IsCorrectAnswer = false,
                        Id = new Random().Next()
                    });

                //%50 chance to add the generated number
                else
                    options.Add(new Option()
                    {
                        Body = (randWeapon.Price * randomNumberToAddOrSubtract).ToString(),
                        IsCorrectAnswer = false,
                        Id = new Random().Next()
                    });
            }

            options.Shuffle();
            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            question.Body = localizedQuestionTemplate.Replace("{WEAPON_NAME}", randWeapon.Name);
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateWeaponMaxDistanceDamageForRandomBodyPartQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(WeaponQuestionType.MAX_DISTANCE_DAMAGE_FOR_RANDOM_BODY_PART);
            var bodyList = EnumHelper<BodyPart>.ConvertToList();
            var randomBodyPart = bodyList[new Random().Next(0, bodyList.Count)];
            var randWeapon = GetRandomWeapon();
            List<Option> options = new List<Option>();

            int correctDamage = randWeapon.WeaponDamages.Where(o => o.DamageToBodyPart == randomBodyPart).First().DamageFromMaxDistance;

            options.Add(new Option()
            {
                Body = correctDamage.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            //subtract and add 1-6 damage point for incorrect options
            int numberOfPointsToMove = 0;

            if (correctDamage <= 2)
                numberOfPointsToMove = 0;
            else if (correctDamage > 2 && correctDamage <= 4)
                numberOfPointsToMove = new Random().Next(1, 2);
            else if (correctDamage > 4 && correctDamage <= 9)
                numberOfPointsToMove = new Random().Next(1, 3);
            else if (correctDamage > 9 && correctDamage < 13)
                numberOfPointsToMove = new Random().Next(1, 4);
            else
                numberOfPointsToMove = new Random().Next(1, 6);

            options.Add(new Option()
            {
                Body = (correctDamage - numberOfPointsToMove).ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            //add 1-6 damage point for incorrect option
            options.Add(new Option()
            {
                Body = (correctDamage + numberOfPointsToMove).ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            //get random damage from the same gun 
            var incorrectDamagesForWeapon = randWeapon.WeaponDamages.Where(o => o.DamageFromMaxDistance != correctDamage);
            var incorrectDamagesForWeaponArr = incorrectDamagesForWeapon.ToArray();
            options.Add(new Option()
            {
                Body = incorrectDamagesForWeaponArr[new Random().Next(0, incorrectDamagesForWeapon.Count())].DamageFromMaxDistance.ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            var localizedBodyPartName = await _localizationService.GetLocalizedEnumAsync(randomBodyPart);
            question.Body = localizedQuestionTemplate.Replace("{WEAPON_NAME}", randWeapon.Name).Replace("{BODY_PART}", localizedBodyPartName);
            options.Shuffle();
            question.Options = options;

            return question;
        }

        public async Task<QuestionDTO> GenerateWeaponMinDistanceDamageForRandomBodyPartQuestionAsync()
        {
            var question = new QuestionDTO();
            var questionTemplate = GetQuestionTemplate(WeaponQuestionType.MIN_DISTANCE_DAMAGE_FOR_RANDOM_BODY_PART);
            var bodyList = EnumHelper<BodyPart>.ConvertToList();
            var randomBodyPart = bodyList[new Random().Next(0, bodyList.Count)];
            var randWeapon = GetRandomWeapon();
            List<Option> options = new List<Option>();

            int correctDamage = randWeapon.WeaponDamages.Where(o => o.DamageToBodyPart == randomBodyPart).First().DamageFromMinDistance;

            options.Add(new Option()
            {
                Body = correctDamage.ToString(),
                IsCorrectAnswer = true,
                Id = new Random().Next()
            });

            //subtract and add 1-6 damage point for incorrect options
            int numberOfPointsToMove = 0;

            if (correctDamage <= 2)
                numberOfPointsToMove = 0;
            else if (correctDamage > 2 && correctDamage <= 4)
                numberOfPointsToMove = new Random().Next(1, 2);
            else if (correctDamage > 4 && correctDamage <= 9)
                numberOfPointsToMove = new Random().Next(1, 3);
            else if (correctDamage > 9 && correctDamage < 13)
                numberOfPointsToMove = new Random().Next(1, 4);
            else
                numberOfPointsToMove = new Random().Next(1, 6);

            options.Add(new Option()
            {
                Body = (correctDamage - numberOfPointsToMove).ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });


            //add 1-6 damage point for incorrect option
            options.Add(new Option()
            {
                Body = (correctDamage + numberOfPointsToMove).ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            //get random damage from the same gun 
            var incorrectDamagesForWeapon = randWeapon.WeaponDamages.Where(o => o.DamageFromMinDistance != correctDamage);
            var incorrectDamagesForWeaponArr = incorrectDamagesForWeapon.ToArray();
            options.Add(new Option()
            {
                Body = incorrectDamagesForWeaponArr[new Random().Next(0, incorrectDamagesForWeapon.Count())].DamageFromMinDistance.ToString(),
                IsCorrectAnswer = false,
                Id = new Random().Next()
            });

            var localizedQuestionTemplate = await _localizationService.GetLocalizedAsync(questionTemplate, q => q.Template);
            var localizedBodyPart = await _localizationService.GetLocalizedEnumAsync(randomBodyPart);
            question.Body = localizedQuestionTemplate.Replace("{BODY_PART}", localizedBodyPart).Replace("{WEAPON_NAME}", randWeapon.Name);
            options.Shuffle();
            question.Options = options;

            return question;
        }
        #endregion

    }
}

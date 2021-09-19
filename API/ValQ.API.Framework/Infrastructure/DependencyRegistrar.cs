using ValQ.Core;
using ValQ.Core.Caching;
using ValQ.Core.Configuration;
using ValQ.Core.Events;
using ValQ.Core.Infrastructure;
using ValQ.Data.Context;
using ValQ.Data.Repository;
using ValQ.Services.Events;
using ValQ.Services.Users;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Services.Localization;
using ValQ.Services.Logging;
using ValQ.Services.Games;
using ValQ.Services.Questions;
using ValQ.Services.Common;

namespace ValQ.API.Framework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 2;

        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            //file provider
            services.AddScoped<IValQFileProvider, ValQFileProvider>();

            //repositories
            services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

            //caching
            if (appSettings.DistributedCacheConfig.Enabled)
            {
                services.AddScoped<ILocker, DistributedCacheManager>();
                services.AddScoped<IStaticCacheManager, DistributedCacheManager>();
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }
            //services
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddScoped<ILogger, Logger>();
            services.AddScoped<IWorkContext, APIWorkContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ICharacterQuestionService, CharacterQuestionService>();
            services.AddScoped<IWeaponQuestionService, WeaponQuestionService>();
            services.AddScoped<ISkillQuestionService, SkillQuestionService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IOriginService, OriginService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IQuestionTemplateService, QuestionTemplateService>();
            services.AddScoped<ICharacterService, CharacterService>();

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);
        }
    }
}

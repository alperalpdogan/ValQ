using ValQ.API.Framework.Infrastructure.Extensions;
using ValQ.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using ValQ.API.Framework.Filters;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ValQ.API.Framework.Infrastructure
{
    public class APIStartup : IValQStartup
    {
        public int Order => 3;

        public void Configure(IApplicationBuilder application)
        {
            ////use HTTP session
            //application.UseSession();
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add distributed cache
            services.AddDistributedCache();
        }
    }
}

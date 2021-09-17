using ValQ.Core.Infrastructure;
using ValQ.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Data
{
    public class DbStartup : IValQStartup
    {
        public int Order => 1;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ValQDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ValQ")));
        }
    }
}

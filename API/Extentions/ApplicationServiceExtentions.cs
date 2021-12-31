using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection addApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>{
               options.UseSqlite (config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
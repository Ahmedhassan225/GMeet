using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions
{
    public static class IdentityServiceExterntions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config){
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>{
                option.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });

            return services;

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions
{
    public static class IdentityServiceExterntions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config){

            services.AddIdentityCore<AppUser>(opt =>{
                opt.Password.RequireNonAlphanumeric = false;
                
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>() 
                .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>{
                option.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });

            services.AddAuthorization(opt=>{
                opt.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
                opt.AddPolicy("ModiratePhotoRole", policy=>policy.RequireRole("Admin", "Moderator"));
            });

            return services;

        }
    }
}
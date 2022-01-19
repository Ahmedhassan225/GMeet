using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class logUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContent = await next();

            var username = resultContent.HttpContext.User.GetUserName();
            var repo  = resultContent.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByUserNameAsync(username);

            // var identity = new System.Security.Principal.GenericIdentity(user.UserName);
            // var principal = new GenericPrincipal(identity, new string[0]);
            // context.HttpContext.User = principal;
            // System.Threading.Thread.CurrentPrincipal = principal;

            if (!resultContent.HttpContext.User.Identity.IsAuthenticated) return;

            
            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync();
        }
    }
}
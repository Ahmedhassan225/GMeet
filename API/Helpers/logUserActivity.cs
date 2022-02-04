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
            var unitOfWork  = resultContent.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await unitOfWork.userRepository.GetUserByUserNameAsync(username);
            if (!resultContent.HttpContext.User.Identity.IsAuthenticated) 
                return;
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.Complete();
        }
    }
}
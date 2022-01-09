using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class ClaimsPrincapleExtention
    {
        public static string GetUserName(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;   
        }
    }
}
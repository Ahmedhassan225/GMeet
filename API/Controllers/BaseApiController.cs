using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(logUserActivity))]
    [ApiController]
    [Route("API/[controller]")]    public class BaseApiController : ControllerBase
    {
        
    }
}
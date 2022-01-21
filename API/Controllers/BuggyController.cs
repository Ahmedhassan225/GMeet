using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context){
            _context = context;
            
        }

        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return Unauthorized("secret text");
        }
        
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
            var thing = _context.Users.Find(-1);

            if (thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
             var temp = _context.Users.Find(-1);
             var tempToReturn = temp.ToString(); //return null refrence exeption

             return tempToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest();
        }
    }
}
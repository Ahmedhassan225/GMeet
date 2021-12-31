using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] //Validation that it is not Null
        public string UserName { get; set; }
        [Required] // we Can add more validation like email or phone or re ...
        public string PassWord { get; set; }
    }
}
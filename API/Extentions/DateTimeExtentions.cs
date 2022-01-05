using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateTime dob){
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if(dob.Date > today.Date) age--;
            return age;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiExeptions
    {
        public ApiExeptions(int statuesCode, string message = null, string details = null)
        {
            StatuesCode = statuesCode;
            Message = message;
            Details = details;
        }

        public int StatuesCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
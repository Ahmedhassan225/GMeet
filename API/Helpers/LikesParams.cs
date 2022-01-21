using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LikesParams : PaginationParam
    {
        public int UserId { get; set; }
        public string Pridecate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParam
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; }

        private int _pageSize = 10; //def value

        public int PageSize{
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
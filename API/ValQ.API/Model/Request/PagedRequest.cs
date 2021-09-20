using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValQ.API.Model.Request
{
    public class PagedRequest
    {
        private const int _maxPageSize = 25;
        private int _pageSize;

        public int PageNumber { get; set; }

        public int PageSize 
        {
            get 
            {
                return _pageSize;
            }
            set 
            {
                _pageSize = value > _maxPageSize ? _maxPageSize : value;
            }
        }
    }
}

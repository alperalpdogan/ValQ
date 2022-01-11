using System.Collections.Generic;

namespace ValQ.API.Model.Response
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

    }
}

using System.Collections.Generic;
using System.Linq;

namespace ProjectNameApi.Entities.Responses
{
    public class PagedListResult<T>
    {
        public int Count { get; set; }
        public List<T> Result { get; set; }

        public PagedListResult()
        {
        }

        public PagedListResult(IEnumerable<T> result, int count)
        {
            Result = result.ToList();
            Count = count;
        }

        public PagedListResult(List<T> result, int count)
        {
            Result = result;
            Count = count;
        }
    }
}
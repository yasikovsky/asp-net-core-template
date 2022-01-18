using System.Collections.Generic;

namespace ProjectNameApi.Entities.Responses
{
    public class ValidationError
    {
        public string Field { get; set; }
        public List<string> Errors { get; set; }
    }
}
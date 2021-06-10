using System.Collections.Generic;


namespace SocialCode.API.Requests
{
    public class PaginatedResult<T>
    {
        public int Offset { get; set; }
        
        public int Limit { get; set;  }
        public IEnumerable<T> Items { get; set; }
        public PaginatedResult()
        {
            
        }
        
    }
}
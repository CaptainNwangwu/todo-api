using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DTOs
{
    /* The class PaginatedResponse<T> represents a paginated response containing a list of items, page
    number, limit per page, and total number of items. */
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
    }
}
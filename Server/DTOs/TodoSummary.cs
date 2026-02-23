using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DTOs
{
    public class TodoSummary
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; } = string.Empty;
    }
}


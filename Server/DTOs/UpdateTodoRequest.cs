using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.DTOs
{
    public class UpdateTodoRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TodoStatus? Status { get; set; }
    }
}

// *TODO: Incorporate Todo DTOs into API Controller
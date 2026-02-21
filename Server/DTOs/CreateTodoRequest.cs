using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class CreateTodoRequest
    {
        [Required]
        [StringLength(255)]
        public required string Title { get; set; }

        [StringLength(255)]
        public string? Description { get; set; } = string.Empty;
    }
}
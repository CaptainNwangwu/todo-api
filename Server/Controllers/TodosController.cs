using Server.Data;
using Server.DTOs;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController(TodoDbContext db) : ControllerBase
    {
        private readonly TodoDbContext _db = db;
    }
}
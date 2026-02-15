using Server.DTOs;
using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(TodoDbContext db) : ControllerBase
    {
        private readonly TodoDbContext _db = db;


        /// <summary>
        /// This C# function retrieves a user from a database by their ID and returns either the user object or
        /// a "Not Found" response.
        /// </summary>
        /// <param name="id">The `id` parameter in the `GetUserByID` method represents the unique identifier of
        /// the user that you want to retrieve from the database. This method is an HTTP GET endpoint that takes
        /// an integer `id` as a route parameter in the URL to fetch the user with the corresponding ID from
        /// the</param>
        /// <returns>
        /// If the user with the specified `id` is found in the database, the `Ok` response with the user object
        /// will be returned. If the user is not found (i.e., `null`), a `NotFound` response will be returned.
        /// </returns>
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // TODO: Implement Registration Logic
            // return Ok(new { message = "Registration Endpoint Access Successful"});

            // ===== Input Validation ===== /
            // * Uniqueness of Email * //
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { error = "Email already exists" });
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password //TODO: Create HASH for password storage
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            var auth_response = new AuthResponse
            {
                Name = request.Name,
                Email = request.Email,
                //TODO: Implement JWT to return token
            };
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, auth_response);
        }
    }

}

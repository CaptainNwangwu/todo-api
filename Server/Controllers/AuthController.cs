using Server.Data;
using Server.DTOs;
using Server.Models;
using Server.Services;
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
        /// The Register method in C# handles user registration by validating input, checking email uniqueness,
        /// hashing the password, and saving the user to the database.
        /// </summary>
        /// <param name="RegisterRequest">RegisterRequest is a class that likely contains properties for the
        /// data needed to register a user, such as Name, Email, and Password. It is used as a parameter in the
        /// Register method to receive the registration data from the client.</param>
        /// <param name="IPasswordHasher">The `IPasswordHasher` interface is typically used for hashing
        /// passwords securely in ASP.NET Core applications. In your code snippet, you are injecting an
        /// implementation of the `IPasswordHasher` interface named `BCryptPasswordHasher` using the
        /// `[FromServices]` attribute.</param>
        /// <returns>
        /// The Register method is returning a response with the newly registered user's information, including
        /// their ID, name, and email. The response is created using the RegisterResponse model and is returned
        /// with a status code of 201 (Created) using the CreatedAtAction method.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request,
        [FromServices] IPasswordHasher BCryptPasswordHasher)
        {
            // * Verify Uniqueness of Email
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { error = "Email already exists" });
            }

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                //?: Check to see if Hashing works properly
                Password = BCryptPasswordHasher.HashPassword(request.Password)

            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
            var registrationResponse = new AuthResponse
            {
                Name = request.Name,
                Email = request.Email,
            };
            return CreatedAtAction(
            nameof(UsersController.GetUserById),
            "Users",
            new { id = newUser.Id },
            registrationResponse);
        }

        //TODO: || ========== Write Login Handler ========== ||

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request,
        [FromServices] ITokenService JwtTokenService)
        {
            //TODO: Incorporate JWT into Login Authentication
            // var user =
        }
    }
}

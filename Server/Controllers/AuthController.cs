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

            // * Verify Password i
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


        /// <summary>
        /// The Login method is a C# method for handling user login requests, verifying credentials,
        /// generating a JWT token, and returning a response.
        /// </summary>
        /// <param name="LoginRequest">The `LoginRequest` parameter in the `Login` method represents the data
        /// model for the login request. It likely contains properties such as `Email` and `Password` that are
        /// submitted by the user when attempting to log in to the system. This parameter is used to bind the
        /// incoming login request data</param>
        /// <param name="ITokenService">ITokenService is a service interface that provides functionality related
        /// to generating and validating JSON Web Tokens (JWTs) for user authentication and authorization in the
        /// application. It typically includes methods for generating tokens based on user information and
        /// validating tokens during authentication processes.</param>
        /// <param name="IPasswordHasher">The `IPasswordHasher` interface in the code snippet is used for
        /// hashing and verifying passwords securely. In this case, it seems to be specifically using the
        /// `BCryptPasswordHasher` implementation for hashing and verifying passwords using the BCrypt
        /// algorithm.</param>
        /// <returns>
        /// The Login method is returning an IActionResult. If the login is successful, it returns an Ok
        /// response with a message "Login successful" and a Data object containing the user's name, email, and
        /// a generated token. If the login is unsuccessful (due to invalid email or password), it returns an
        /// Unauthorized response with an error message "Invalid email or password".
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request,
        [FromServices] ITokenService JwtTokenService,
        [FromServices] IPasswordHasher BCryptPasswordHasher)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCryptPasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                return Unauthorized(new { Error = "Invalid email or password" });
            }

            var generatedToken = JwtTokenService.GenerateToken(user);
            var loginResponse = new AuthResponse
            {
                Name = user.Name,
                Email = user.Email,
                Token = generatedToken
            };

            return Ok(new
            {
                Message = "Login successful",
                Data = loginResponse
            });
        }
    }
}


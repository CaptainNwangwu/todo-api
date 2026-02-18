using Server.Data;
using Server.DTOs;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(TodoDbContext db) : ControllerBase
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        /// <summary>
        /// This C# function retrieves a user from a database based on their email address.
        /// </summary>
        /// <param name="email">The `HttpGet` attribute in the code snippet indicates that this method is a
        /// controller action that responds to HTTP GET requests. The route template specified in the attribute
        /// is `users/email/{email}`, which means this action can be accessed using a URL like
        /// `/users/email/example@example.com`, where `example@example</param>
        /// <returns>
        /// If a user with the specified email is found in the database, the user object will be returned with a
        /// status code of 200 (OK). If no user is found with the specified email, a status code of 404 (Not
        /// Found) will be returned.
        /// </returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _db.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            return user is not null ? Ok(user) : NotFound();
        }


        /// <summary>
        /// This C# function retrieves all users from the database and returns them as a response.
        /// </summary>
        /// <returns>
        /// A list of all users from the database is being returned.
        /// </returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }



        /// <summary>
        /// This C# function deletes a user by their ID from a database and returns a success message along with
        /// the deleted user data.
        /// </summary>
        /// <param name="id">The `id` parameter in the `DeleteUserById` method represents the unique identifier
        /// of the user that you want to delete from the database. This identifier is used to locate the
        /// specific user record that needs to be removed.</param>
        /// <returns>
        /// The DeleteUserById method is returning an IActionResult. If the user with the specified id is found
        /// and successfully deleted, it returns an Ok result with a success message and the deleted user's
        /// data. If the user is not found, it returns a NotFound result.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            string successMsg = "Successfully deleted User.";
            return Ok(new
            {
                Messsage = successMsg,
                Data = user
            });
        }

    }
}
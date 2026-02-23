/* This code snippet is a C# class representing a controller for handling Todo-related operations in a
server-side application. Here is a breakdown of what the code is doing: */
using Server.Data;
using Server.DTOs;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace Server.Controllers
{
    /// <summary>
    /// Controller for managing Todo items.
    /// Provides endpoints for CRUD operations on todos with user authorization and ownership validation.
    /// All endpoints require authentication.
    /// </summary>
    [ApiController]
    [Route("/todos")]
    [Authorize]
    public class TodosController(TodoDbContext db) : ControllerBase
    {
        private readonly TodoDbContext _db = db;


        private int? GetAuthenticatedUserId()
        {
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return raw is not null ? int.Parse(raw) : null;
        }
        /// <summary>
        /// Retrieves a single Todo item by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the todo item to retrieve.</param>
        /// <returns>
        /// An Ok response containing the todo item if found.
        /// A NotFound response if the todo item does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized();

            var todo = await _db.Todos.FindAsync(id);
            if (todo is null)
                return NotFound("No todo item with the given id exists");

            if (userId != todo.UserId)
                return Forbid("Forbidden");

            return Ok(todo);
        }


        /// <summary>
        /// Retrieves all Todo items for the authenticated user with pagination support.
        /// </summary>
        /// <param name="page">The page number for pagination (default: 1). Must be greater than 0.</param>
        /// <param name="limit">The number of items per page (default: 10). Determines page size.</param>
        /// <returns>
        /// An Ok response containing paginated todo summaries with metadata.
        /// Returns only lightweight TodoSummary objects (Id, Title, Description) for efficiency.
        /// An Unauthorized response if the user is not authenticated.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTodos(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] TodoStatus? status = null,
            [FromQuery] string? sortBy = "title",
            [FromQuery] string? sortOrder = "asc")
        {
            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized();


            var total = await _db.Todos.CountAsync(t => t.UserId == userId);

            int paginationOffset = (page - 1) * limit;

            // Filtering & Sorting Query (Start)
            var query = _db.Todos.Where(t => t.UserId == userId);

            if (status is not null)
            {
                query = query.Where(t => t.Status == status);
            }

            query = sortBy switch
            {
                "title" => sortOrder == "desc"
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),
                "status" => sortOrder == "desc"
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),
                "created" => sortOrder == "desc"
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),
                _ => query.OrderBy(t => t.Title)  // default
            };

            var todos = await query
                .Select(t => new TodoSummary { Id = t.Id, Title = t.Title, Description = t.Description ?? string.Empty })
                .Skip(paginationOffset)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                Data = todos,
                Page = page,
                Limit = limit,
                Total = total
            });
        }


        /// <summary>
        /// Creates a new Todo item for the authenticated user.
        /// Validates that the todo title is unique per user before creation.
        /// </summary>
        /// <param name="newTodoReq">The request containing the title and description for the new todo.</param>
        /// <returns>
        /// A CreatedAtAction response (HTTP 201) with the newly created todo and its location.
        /// A Conflict response if a todo with the same title already exists for this user.
        /// An Unauthorized response if the user is not authenticated.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoRequest newTodoReq)
        {
            var userId = GetAuthenticatedUserId();
            if (userId is null) return Unauthorized();
            // Verify uniqueness of todo title per user to prevent duplicate titles
            if (await _db.Todos.AnyAsync(t => t.UserId == userId && t.Title == newTodoReq.Title))
            {
                return Conflict(
                    new
                    {
                        Error = "Title is unavailable."
                    });
            }


            var newTodo = new Todo
            {
                UserId = (int)userId,
                Title = newTodoReq.Title,
                Description = newTodoReq.Description,
            };

            _db.Todos.Add(newTodo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetTodoById),
                "Todos",
                new { id = newTodo.Id },
                newTodo);
        }

        /// <summary>
        /// Deletes a Todo item by its ID.
        /// Only the owner of the todo can delete it.
        /// </summary>
        /// <param name="id">The unique identifier of the todo to delete.</param>
        /// <returns>
        /// An Ok response with the deleted todo item data.
        /// A NotFound response if the todo item does not exist.
        /// An Unauthorized response if the user is not authenticated.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoById(int id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized();
            var todo = await _db.Todos.FindAsync(id);
            if (todo is null)
            {
                return NotFound(
                    new
                    {
                        Error = "No todo item with the given id exists"
                    }
                );
            }

            if (todo.UserId != userId)
                return Forbid("Forbidden");

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return Ok(new
            {
                Message = "Deletion Successful",
                Data = todo
            });
        }

        /// <summary>
        /// Updates a Todo item with the provided fields.
        /// Supports partial updates - only provided fields are modified.
        /// Only the owner of the todo can update it.
        /// </summary>
        /// <param name="id">The unique identifier of the todo to update.</param>
        /// <param name="request">The request containing optional fields to update (Title, Description, Status).
        /// Null/missing fields are not updated.</param>
        /// <returns>
        /// An Ok response with the updated todo item data.
        /// A NotFound response if the todo item does not exist.
        /// An Unauthorized response if the user is not authenticated.
        /// A Forbid response if the user does not own the todo item.
        /// </returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateTodoById(int id, UpdateTodoRequest request)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo is null)
            {
                return NotFound(
                    new
                    {
                        Error = "No todo item with the given id exists"
                    }
                );
            }
            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized();

            if (userId != todo.UserId)
            {
                return Forbid("Forbidden");
            }

            // Update only the fields that were provided in the request.
            // Nullable DTO fields indicate "optional" (not provided), while non-null values indicate the user wants to update.
            if (request.Title is string title)
            {
                todo.Title = title;
            }

            if (request.Description is string description)
            {
                todo.Description = description;
            }

            if (request.Status is TodoStatus status)
            {
                todo.Status = status;
            }

            todo.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(new
            {
                Message = "Update Successful",
                Data = todo
            });
        }
    }
}
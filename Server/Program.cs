using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

/* 
*   ====== 1. CREATE BUILDER ======
    Sets up the web application with the default configuration
*/
var builder = WebApplication.CreateBuilder(args);

/*
*   ====== 2. REGISTER SERVICES (Dependency Injection) ======
*   Register services that will be injected into endpoints
*/

// Register DBContext with Postgres
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add API Documentation (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")  // Vue dev server)
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});



/*
*   ====== 3. BUILD THE APP ======
*/
var app = builder.Build();

/*
*   ====== 4. CONFIGURE MIDDLEWARE (Request Pipeline) ======
*/

// Enable Swagger (only in Development Mode)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Health check endpoint
app.MapGet("/", () => "Todo API is running!");

// Get All Users
app.MapGet("/users", async (TodoDbContext db) =>
{
    return await db.Users.ToListAsync();
});

// Get User by ID
app.MapGet("/users/{id}", async (int id, TodoDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// Create New User
app.MapPost("/users", async (User user, TodoDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});


app.Run();
/* This code snippet is setting up a web application using C# with ASP.NET Core. Here is a breakdown of
what each part of the code is doing: */
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;
using Server.Controllers;

/* 
*   ====== 1. CREATE BUILDER ======
    Sets up the web application with the default configuration
*/
var builder = WebApplication.CreateBuilder(args);

/*
*   ====== 2. REGISTER SERVICES (Dependency Injection) ======
*   Register services that will be injected into endpoints
*/

// Register API Controller
builder.Services.AddControllers();

// Register Token Service for Authentication
builder.Services.AddScoped<ITokenService, JwtTokenService>();

/* TODO: Register 
JWTbuilder.Services.AddAuthentication().AddJwtBearer
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/", () => "Todo API is running!");

// TODO: Get All Users
app.Run();
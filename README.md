# Todo API

![.NET](https://img.shields.io/badge/.NET-8.0-512bd4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791)
![JWT](https://img.shields.io/badge/Auth-JWT-orange)
![License](https://img.shields.io/badge/license-MIT-blue)

**Project URL:** This project is based on the [Todo List API project from roadmap.sh](https://roadmap.sh/projects/todo-list-api)

## Quick Start

**Requirements:** .NET 8.0 SDK, PostgreSQL 16+

```bash
# Set up secrets
cd Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=todoapi;Username=postgres;Password=yourpassword"
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-at-least-32-characters-long"

# Apply database migrations
dotnet ef database update

# Run the server
dotnet run
```

API available at `http://localhost:5043`. Swagger UI at `http://localhost:5043/swagger` (Development mode).

## Features

- **JWT Authentication**
  - User registration with BCrypt password hashing
  - Login with JWT token generation
  - Protected endpoints with ownership validation

- **Full CRUD for Todos**
  - Create, read, update, and delete todo items
  - Partial updates via PATCH (only modify provided fields)
  - Unique title enforcement per user

- **Pagination, Filtering & Sorting**
  - Configurable page size and page number
  - Filter by todo status (Created, InProgress, Done)
  - Sort by title, status, or creation date (ascending/descending)

- **Security**
  - Password hashing with configurable BCrypt work factor
  - JWT token validation with issuer/audience/signature checks
  - Ownership enforcement — users can only access their own todos
  - Sensitive configuration stored in user-secrets (not in source)
  - Input validation via data annotations on request DTOs

## Tech Stack

- **ASP.NET Core 8.0** — Web framework with MVC controllers
- **Entity Framework Core 8.0** — ORM with Fluent API configuration
- **PostgreSQL** — Relational database via Npgsql provider
- **BCrypt.Net** — Password hashing
- **JWT Bearer Authentication** — Token-based auth via Microsoft.AspNetCore.Authentication.JwtBearer
- **Swagger/OpenAPI** — API documentation via Swashbuckle

## Getting Started

### Prerequisites

1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Install [PostgreSQL](https://www.postgresql.org/download/)
3. Create a database:
   ```sql
   CREATE DATABASE todoapi;
   ```

### Configuration

The project uses .NET user-secrets for sensitive configuration. Initialize and set required secrets:

```bash
cd Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=todoapi;Username=postgres;Password=yourpassword"
dotnet user-secrets set "JwtSettings:SecretKey" "$(openssl rand -base64 64)"
```

Non-sensitive configuration lives in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Issuer": "todo-api",
    "Audience": "todo-api",
    "ExpirationHours": 24
  },
  "SecuritySettings": {
    "HashWorkFactor": 12
  }
}
```

### Running the Server

```bash
cd Server

# Apply database migrations
dotnet ef database update

# Start the server
dotnet run
```

The API runs on `http://localhost:5043` by default.

### Running with Swagger

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

Navigate to `http://localhost:5043/swagger` to explore and test endpoints interactively.

## API Endpoints

### Authentication (Public)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/register` | Register a new user |
| POST | `/login` | Login and receive JWT token |

### Users (Protected — requires JWT)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users` | Get all users |
| GET | `/users/{id}` | Get user by ID |
| DELETE | `/users/{id}` | Delete user by ID |

### Todos (Protected — requires JWT)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/todos` | Get all todos (paginated, filterable, sortable) |
| GET | `/todos/{id}` | Get todo by ID |
| POST | `/todos` | Create a new todo |
| PATCH | `/todos/{id}` | Partial update (title, description, and/or status) |
| DELETE | `/todos/{id}` | Delete todo by ID |

### Query Parameters for GET /todos

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `limit` | int | 10 | Items per page |
| `status` | string | (none) | Filter by status: `Created`, `InProgress`, `Done` |
| `sortBy` | string | `title` | Sort field: `title`, `status`, `created` |
| `sortOrder` | string | `asc` | Sort direction: `asc`, `desc` |

### Request & Response Examples

**Register:**
```json
POST /register
{
  "name": "CJ Nwangwu",
  "email": "cj@example.com",
  "password": "SecurePass123!"
}

Response (201):
{
  "name": "CJ Nwangwu",
  "email": "cj@example.com",
  "token": ""
}
```

**Login:**
```json
POST /login
{
  "email": "cj@example.com",
  "password": "SecurePass123!"
}

Response (200):
{
  "message": "Login successful",
  "data": {
    "name": "CJ Nwangwu",
    "email": "cj@example.com",
    "token": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

**Create Todo:**
```json
POST /todos
Authorization: Bearer <token>
{
  "title": "Finish JWT implementation",
  "description": "Wire up authentication middleware"
}

Response (201):
{
  "id": 1,
  "userId": 2,
  "title": "Finish JWT implementation",
  "description": "Wire up authentication middleware",
  "status": "Created",
  "createdAt": "2026-02-21T17:00:00Z",
  "updatedAt": "2026-02-21T17:00:00Z"
}
```

**Get Todos (with filtering, sorting, pagination):**
```
GET /todos?status=InProgress&sortBy=created&sortOrder=desc&page=1&limit=5
Authorization: Bearer <token>

Response (200):
{
  "data": [
    { "id": 1, "title": "Finish JWT", "description": "..." }
  ],
  "page": 1,
  "limit": 5,
  "total": 12
}
```

**Partial Update:**
```json
PATCH /todos/1
Authorization: Bearer <token>
{
  "status": "Done"
}

Response (200):
{
  "message": "Update Successful",
  "data": { ... }
}
```

### Authentication

All protected endpoints require a JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Obtain a token by logging in. Tokens expire after 24 hours (configurable).

## Project Structure

```
todo-api/
├── Server/
│   ├── Controllers/
│   │   ├── AuthController.cs        # Register, Login (public)
│   │   ├── TodosController.cs       # Todo CRUD (protected, ownership enforced)
│   │   └── UsersController.cs       # User CRUD (protected)
│   ├── Data/
│   │   └── TodoDbContext.cs          # EF Core DbContext + Fluent API config
│   ├── DTOs/
│   │   ├── AuthResponse.cs           # Login/Register response
│   │   ├── RegisterRequest.cs        # Registration input
│   │   ├── LoginRequest.cs           # Login input
│   │   ├── CreateTodoRequest.cs      # Todo creation input
│   │   ├── UpdateTodoRequest.cs      # Partial update input (nullable fields)
│   │   ├── TodoSummary.cs            # Lightweight todo projection
│   │   └── PaginatedResponse.cs      # Generic pagination wrapper
│   ├── Models/
│   │   ├── User.cs                   # User entity
│   │   └── Todo.cs                   # Todo entity + TodoStatus enum
│   ├── Services/
│   │   ├── ITokenService.cs          # Token generation interface
│   │   ├── JwtTokenService.cs        # JWT implementation
│   │   ├── IPasswordHasher.cs        # Password hashing interface
│   │   └── BCryptPasswordHasher.cs   # BCrypt implementation
│   ├── Migrations/                   # EF Core migration files
│   ├── Program.cs                    # App startup, DI, middleware pipeline
│   ├── appsettings.json              # Non-sensitive configuration
│   ├── appsettings.Development.json  # Dev-specific overrides
│   ├── server.csproj                 # Project file + NuGet packages
│   └── server.http                   # API test requests
├── Client/                           # (Future frontend)
├── todo-api.sln
└── README.md
```

## Database Schema

```
Users                          Todos
┌──────────────┐              ┌──────────────────┐
│ Id (PK)      │─────────────<│ Id (PK)          │
│ Name         │              │ UserId (FK)      │
│ Email (UQ)   │              │ Title            │
│ Password     │              │ Description      │
│ CreatedAt    │              │ Status           │
│ UpdatedAt    │              │ CreatedAt        │
└──────────────┘              │ UpdatedAt        │
                              └──────────────────┘
```

- One-to-Many: User → Todos (cascade delete)
- Unique index on User.Email
- TodoStatus stored as string (Created, InProgress, Done)

## Design Decisions

### Architecture
- **Controllers are thin** — handle HTTP concerns only (routing, status codes, request/response shapes)
- **Services for business logic** — TokenService and PasswordHasher are injected via DI, decoupled from controllers
- **Interface-driven DI** — `ITokenService`/`IPasswordHasher` enable swapping implementations without changing consumers
- **DTOs separate concerns** — request DTOs validate input, response DTOs control what's exposed, entities stay internal

### Security
- **Passwords never stored in plaintext** — BCrypt with configurable work factor
- **Secrets stored in user-secrets** — connection string and JWT signing key stay out of source control
- **Ownership validation on every protected endpoint** — users can only access their own todos
- **Intentionally vague auth errors** — "Invalid email or password" doesn't reveal which part was wrong

### API Design
- **PATCH for partial updates** — nullable DTO fields distinguish "not provided" from "set to value"
- **Pagination metadata in response** — total count uses a separate query for accuracy
- **Filtering and sorting happen in the database** — IQueryable is built conditionally, not materialized then filtered in memory

## Development

### Useful Commands

```bash
# Build
dotnet build

# Run with hot reload
dotnet watch

# Run in development mode (enables Swagger)
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Add a migration after model changes
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# View stored secrets
dotnet user-secrets list
```

### Testing with .http Files

A `server.http` file is included for quick endpoint testing in VS Code (requires REST Client extension):

```http
@server_HostAddress = http://localhost:5043
@token = paste-token-here-after-login

### Login
POST {{server_HostAddress}}/login
Content-Type: application/json

{
    "Email": "cj@example.com",
    "Password": "SecurePass123!"
}

### Get Todos (protected)
GET {{server_HostAddress}}/todos?page=1&limit=10
Authorization: Bearer {{token}}
```

## What I Learned

This project helped me practice:
- Building RESTful APIs with ASP.NET Core MVC controllers
- Implementing JWT authentication end-to-end (generation, middleware validation, claims extraction)
- Database design and EF Core Fluent API configuration
- Dependency injection with interface-driven service design
- Secure password hashing with BCrypt
- Pagination, filtering, and sorting with IQueryable (deferred execution)
- Proper separation of concerns (Controllers → Services → Data)
- Managing secrets with .NET user-secrets
- Linux development environment setup (WSL + PostgreSQL)

## License

This project is open source and available for educational purposes.

## Acknowledgments

- Project based on [roadmap.sh Todo List API project](https://roadmap.sh/projects/todo-list-api)
- Backend powered by ASP.NET Core 8.0 and PostgreSQL

---

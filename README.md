# BudgetApp — Backend API

A **.NET 8 Web API** built with Clean Architecture, CQRS, MediatR, and Entity Framework Core. This is the backend repository for BudgetApp — a fullstack budgeting application where users can manage monthly budgets, categories, and transactions.

> The frontend repository (React) connects to this API. See the [frontend repo](#) for setup instructions.

---

## Table of Contents

- [Technologies Used](#technologies-used)
- [Architecture Overview](#architecture-overview)
- [Data Models & Relationships](#data-models--relationships)
- [API Endpoints](#api-endpoints)
- [Prerequisites](#prerequisites)
- [Setup & Running Locally](#setup--running-locally)
- [Environment Variables](#environment-variables)
- [Running Tests](#running-tests)
- [Project Structure](#project-structure)
- [Authentication & Authorization](#authentication--authorization)
- [Design Decisions](#design-decisions)

---

## Technologies Used

| Technology | Purpose |
|---|---|
| .NET 8 / ASP.NET Core | Web API framework |
| Entity Framework Core | ORM and database migrations |
| PostgreSQL | Relational database |
| MediatR | CQRS — decouples controllers from business logic |
| AutoMapper | Maps between entities, DTOs, and commands |
| FluentValidation | Input validation via pipeline behaviour |
| BCrypt.Net | Password hashing |
| JWT Bearer | Authentication tokens |
| Swagger / Swashbuckle | API documentation |
| NUnit | Unit testing framework |

---

## Architecture Overview

The solution follows **Clean Architecture** with four separate projects and a test project:

```
BudgetApp.API            → Controllers, Program.cs, entry point
BudgetApp.Application    → Commands, Queries, DTOs, Interfaces, Validators, Mappings
BudgetApp.Domain         → Entities (pure C# classes, no dependencies)
BudgetApp.Infrastructure → Repositories, AppDbContext, JwtTokenService, Migrations
BudgetApp.Tests          → Unit tests for Application layer handlers
```

**Dependency direction:** API → Application → Domain. Infrastructure implements interfaces defined in Application. Nothing in Application or Domain knows about the database or HTTP.

**CQRS with MediatR:** Every action is expressed as a `Command` (writes data) or a `Query` (reads data). Controllers send these through MediatR, which routes them to the correct handler — keeping controllers thin and business logic isolated and testable.

**Pipeline Behaviour:** A `ValidationBehaviour` runs automatically before every handler. It collects all FluentValidation validators for the incoming request and throws a `ValidationException` if any rule fails — before the handler is ever called.

**Dependency Injection:** Each layer has its own `DependencyInjection.cs` file. `Program.cs` calls `AddApplication()` and `AddInfrastructure()` — nothing is registered directly in `Program.cs`.

---

## Data Models & Relationships

```
User ──< Budget ──< Category ──< Transaction
```

| Entity | Key Fields |
|---|---|
| `User` | Id, Email, PasswordHash, Role, CreatedAt |
| `Budget` | Id, UserId (FK), Name, Month, Year, TotalAmount |
| `Category` | Id, BudgetId (FK), Name, AllocatedAmount, CurrentBalance, IsWeekly, WeeklyAmount |
| `Transaction` | Id, CategoryId (FK), Amount, Type (Income/Expense), Description, Date |

**Relationships:**
- A `User` has many `Budget` records (one per month/year)
- A `Budget` has many `Category` records
- A `Category` has many `Transaction` records

All entities use `Guid` as primary key.

---

## API Endpoints

All endpoints except `/api/auth/*` require a valid JWT token: `Authorization: Bearer <token>`

### Auth
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Create a new user account | Public |
| POST | `/api/auth/login` | Log in and receive a JWT token | Public |

### Budgets
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/budgets/{userId}` | Get all budgets for a user |
| GET | `/api/budgets/single/{id}` | Get a single budget by ID |
| GET | `/api/budgets/summary/{id}` | Get budget summary with category totals |
| GET | `/api/budgets/admin/all` | Get all budgets — **Admin only** |
| POST | `/api/budgets` | Create a new budget |
| PUT | `/api/budgets/{id}` | Update a budget |
| DELETE | `/api/budgets/{id}` | Delete a budget |

### Categories
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/categories/{budgetId}` | Get all categories for a budget |
| GET | `/api/categories/single/{id}` | Get a single category by ID |
| POST | `/api/categories` | Create a new category |
| PUT | `/api/categories/{id}` | Update a category |
| DELETE | `/api/categories/{id}` | Delete a category |

### Transactions
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/transactions/{categoryId}` | Get all transactions for a category |
| GET | `/api/transactions/single/{id}` | Get a single transaction by ID |
| POST | `/api/transactions` | Create a new transaction |
| PUT | `/api/transactions/{id}` | Update a transaction |
| DELETE | `/api/transactions/{id}` | Delete a transaction |

### Users
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/users` | Get all users — **Admin only** |
| GET | `/api/users/{id}` | Get a single user by ID |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) running locally, or a hosted connection string (e.g. Railway)

---

## Setup & Running Locally

**1. Clone the repository**
```bash
git clone <backend-repo-url>
cd group-project-backend
```

**2. Configure the connection string**

Open `BudgetApp.API/appsettings.json` and update the values to match your PostgreSQL instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BudgetAppDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Secret": "YourSecretKeyHere_AtLeast32Characters",
    "Issuer": "BudgetApp",
    "Audience": "BudgetAppUsers",
    "ExpiresInMinutes": 60
  }
}
```

> **Note:** Never commit real credentials. Use `appsettings.Development.json` or environment variables for local overrides.

**3. Run the application**
```bash
cd BudgetApp.API
dotnet run
```

Database migrations are applied automatically on startup. A default **Admin** user is also seeded automatically via `DbInitializer` if one does not already exist.

**4. View API documentation**

Open `http://localhost:5123/swagger` in your browser to explore and test all endpoints interactively.

---

## Environment Variables

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Secret` | Secret key for signing JWT tokens (minimum 32 characters) |
| `Jwt:Issuer` | JWT issuer name |
| `Jwt:Audience` | JWT audience name |
| `Jwt:ExpiresInMinutes` | Token lifetime in minutes |

---

## Running Tests

The test project (`BudgetApp.Tests`) uses **NUnit** with an **in-memory database** — no external database required.

Tests cover all four models and their full CRUD flow, testing handlers directly in the Application layer:

| Test File | Coverage |
|---|---|
| `BudgetHandlerTests.cs` | Create, Read, Update, Delete budgets |
| `CategoryHandlerTests.cs` | Create, Read, Update, Delete categories |
| `TransactionHandlerTests.cs` | Create, Read, Update, Delete transactions |
| `UserHandlerTests.cs` | Create, Read, Update, Delete users |

**Run all tests:**
```bash
cd BudgetApp.Tests
dotnet test
```

---

## Project Structure

```
BudgetApp.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── BudgetsController.cs
│   ├── CategoryController.cs
│   ├── TransactionsController.cs
│   └── UsersController.cs
└── Program.cs

BudgetApp.Application/
├── Behaviours/
│   └── ValidationBehaviour.cs        ← Runs FluentValidation before every handler
├── Features/
│   ├── Auth/
│   │   ├── Commands/                  ← LoginCommand, RegisterCommand + handlers
│   │   └── DTOs/                      ← AuthResponseDto
│   ├── Budgets/
│   │   ├── Commands/                  ← Create, Update, Delete + handlers
│   │   ├── Queries/                   ← GetAll, GetById, GetSummary + handlers
│   │   ├── DTOs/
│   │   └── Validators/
│   ├── Categories/                    ← Same structure as Budgets
│   ├── Transactions/                  ← Same structure as Budgets
│   └── Users/                         ← Same structure as Budgets
├── Interfaces/
│   ├── IRepository.cs                 ← Generic base interface
│   ├── IBudgetRepository.cs
│   ├── ICategoryRepository.cs
│   ├── ITransactionRepository.cs
│   ├── IUserRepository.cs
│   └── IJwtTokenService.cs
├── Mappings/
│   └── MappingProfile.cs              ← AutoMapper: Entity → DTO
└── DependencyInjection.cs             ← Registers MediatR, AutoMapper, FluentValidation

BudgetApp.Domain/
└── Entities/
    ├── User.cs
    ├── Budget.cs
    ├── Category.cs
    └── Transaction.cs

BudgetApp.Infrastructure/
├── Database/
│   └── AppDbContext.cs
├── Repositories/
│   ├── BudgetRepository.cs
│   ├── CategoryRepository.cs
│   ├── TransactionRepository.cs
│   └── UserRepository.cs
├── Migrations/
├── DbInitializer.cs                   ← Seeds Admin user on first startup
├── JwtTokenService.cs
└── DependencyInjection.cs             ← Registers DbContext, Repositories, JwtTokenService

BudgetApp.Tests/
├── BudgetHandlerTests.cs
├── CategoryHandlerTests.cs
├── TransactionHandlerTests.cs
└── UserHandlerTests.cs
```

---

## Authentication & Authorization

- Passwords are hashed with **BCrypt** before storage — plain-text passwords are never saved.
- On login or register, the server issues a **JWT token** containing the user's ID, email, and role.
- All controllers except `AuthController` are decorated with `[Authorize]` — requests without a valid token receive `401 Unauthorized`.
- The admin endpoints use `[Authorize(Roles = "Admin")]` for **Role-Based Access Control (RBAC)**.
- Token settings (secret, issuer, audience, expiry) are configured in `appsettings.json`.

---

## Design Decisions

**CQRS with MediatR** — Separating Commands from Queries keeps each handler small and independently testable. Adding a new feature means adding a new handler without touching existing code.

**Generic Repository (`IRepository<T>`)** — Shared CRUD logic is defined once in a base interface and extended per entity only where needed, reducing duplication across repositories.

**AutoMapper** — Entities are never sent directly to the client. AutoMapper converts them to DTOs, controlling exactly what data is exposed and allowing computed fields (e.g. `AmountSpent = AllocatedAmount - CurrentBalance`) to be calculated cleanly at mapping time.

**Pipeline Behaviour for validation** — Validation runs as part of the MediatR pipeline before any handler executes. Handlers can always assume their input is valid, keeping handler code clean and ensuring consistent `400 Bad Request` responses across all endpoints.

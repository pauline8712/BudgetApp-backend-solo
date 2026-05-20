# group-project-backend

## Local development

The backend uses **PostgreSQL** via `Npgsql.EntityFrameworkCore.PostgreSQL`. You need a running Postgres instance reachable from your machine before the API will start.

Default connection (in `BudgetApp.API/appsettings.json`):

```
Host=localhost;Port=5432;Database=BudgetAppDb;Username=postgres;Password=postgres
```

These are dev placeholders. **Don't commit real credentials.** Override locally with .NET User Secrets:

```powershell
dotnet user-secrets --project BudgetApp.API set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=<port>;Database=BudgetAppDb;Username=<user>;Password=<pwd>"
```

User Secrets auto-load when `ASPNETCORE_ENVIRONMENT=Development` (default in `launchSettings.json`).

> Note: port `5432` may be occupied by other Docker containers on shared dev machines. If yours runs on a different port, just set it via the command above — no code change needed.

Apply the schema:

```powershell
dotnet ef database update --project BudgetApp.Infrastructure --startup-project BudgetApp.API
```

Run the API:

```powershell
dotnet run --project BudgetApp.API
```

Swagger UI: `https://localhost:7057/swagger` (or `http://localhost:5123/swagger`).

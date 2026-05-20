# BudgetApp Backend — Setup Guide

This guide walks you through getting the backend running locally on **Windows** or **macOS**. By the end you'll have:

- A PostgreSQL database running on your machine
- The API talking to that database
- Swagger UI open in your browser

If you get stuck, jump to the [Troubleshooting](#troubleshooting) section at the bottom.

---

## 1. Prerequisites

Install these once. Skip what you already have.

### Both platforms

- **Git** — to clone the repo.
- **.NET 8 SDK** — download from <https://dotnet.microsoft.com/download/dotnet/8.0>. Verify with:
  ```
  dotnet --version
  ```
  You should see a version starting with `8.` or higher.
- A code editor: **VS Code**, **Visual Studio 2022+**, or **JetBrains Rider**.

### PostgreSQL

You can either install Postgres natively **or** run it in Docker. Pick one.

#### Option A — Native install (recommended for beginners)

**Windows:**
1. Download the installer from <https://www.postgresql.org/download/windows/> (use the EDB installer, version 16 or 17).
2. During install, set a password for the `postgres` superuser. **Write it down.**
3. Keep the default port `5432`.
4. Verify after install — open PowerShell and run:
   ```powershell
   & "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -c "SELECT version();"
   ```
   (Adjust the version folder if you installed 16.)

**macOS:**
1. Install via Homebrew:
   ```bash
   brew install postgresql@17
   brew services start postgresql@17
   ```
2. Verify:
   ```bash
   psql postgres -c "SELECT version();"
   ```
3. On Mac, Homebrew Postgres uses your macOS username as the default user with no password. That's fine for local dev.

#### Option B — Docker

If you already have Docker Desktop, this is the cleanest path on either OS:

```
docker run -d --name budgetapp-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=BudgetAppDb -p 5432:5432 postgres:17-alpine
```

If port `5432` is already in use, swap `-p 5432:5432` for `-p 5433:5432` and remember to use port `5433` in the connection string below.

---

## 2. Clone the repo

```
git clone https://github.com/pauline8712/group-project-backend.git
cd group-project-backend
```

---

## 3. Create the database

The migration step in section 5 will create the **tables**, but the **database itself** needs to exist first.

**Windows (native Postgres):**
```powershell
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -c "CREATE DATABASE \"BudgetAppDb\";"
```

**macOS (native Postgres):**
```bash
psql postgres -c 'CREATE DATABASE "BudgetAppDb";'
```

**Docker users:** Skip this step — the database was created by the `POSTGRES_DB` env var when you started the container.

> The double quotes around `BudgetAppDb` matter — they preserve the capital letters.

---

## 4. Set your connection string with User Secrets

The repo ships with placeholder credentials (`postgres`/`postgres`) in `appsettings.json`. **Never put real passwords in that file.** Instead, store them in .NET User Secrets — they live outside the repo on your machine.

Run this from the repo root, replacing `<user>` and `<pwd>` with your actual Postgres credentials:

**Windows (PowerShell):**
```powershell
dotnet user-secrets --project BudgetApp.API set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=BudgetAppDb;Username=<user>;Password=<pwd>"
```

**macOS (Terminal):**
```bash
dotnet user-secrets --project BudgetApp.API set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=BudgetAppDb;Username=<user>;Password=<pwd>"
```

Typical values:

| Setup | Username | Password |
|---|---|---|
| Windows native install | `postgres` | the password you set during install |
| macOS Homebrew | your macOS username (e.g. `pauline`) | *leave empty: `Password=`* |
| Docker (this guide) | `postgres` | `postgres` |

Verify it was stored:
```
dotnet user-secrets --project BudgetApp.API list
```

---

## 5. Apply the schema (migrations)

Install the EF Core CLI (one-time, global):

```
dotnet tool install --global dotnet-ef
```

If you already have it, you can update it instead with `dotnet tool update --global dotnet-ef`.

Run the migration:

```
dotnet ef database update --project BudgetApp.Infrastructure --startup-project BudgetApp.API
```

You should see messages like `Applying migration '20260520100535_InitialCreate'.` and finally `Done.`. This creates the `Users`, `Budgets`, `Categories`, and `Transactions` tables.

---

## 6. Run the API

```
dotnet run --project BudgetApp.API
```

Watch the console for a line like:

```
Now listening on: https://localhost:7057
Now listening on: http://localhost:5123
```

Open <https://localhost:7057/swagger> in your browser. You should see the Swagger UI with all the endpoints.

> First time only: your browser may warn about the dev certificate. Run `dotnet dev-certs https --trust` once to trust it.

---

## 7. Smoke test

In Swagger:

1. Expand `POST /api/users`, click **Try it out**, paste a body like:
   ```json
   {
     "email": "test@example.com",
     "passwordHash": "fakehash",
     "role": "User"
   }
   ```
   Click **Execute**. You should get a `201 Created` with a generated `id`.
2. Expand `GET /api/users` and click **Execute**. Your new user should appear in the response.

If both work, you're done — the API is talking to PostgreSQL.

---

## Troubleshooting

### `Npgsql.NpgsqlException: Connection refused`
Postgres isn't running, or it's on a different port.
- Native install: make sure the service is started. Windows: `services.msc` → look for `postgresql-x64-17`. Mac: `brew services list`.
- Docker: `docker ps` — your container should say `Up`. If not, `docker start budgetapp-postgres`.
- Wrong port? Update your user-secret with the correct port.

### `28P01: password authentication failed for user "postgres"`
The password in your user-secret doesn't match what Postgres expects. Re-run the `dotnet user-secrets set` command from step 4 with the correct password.

### `3D000: database "BudgetAppDb" does not exist`
You skipped step 3. Go back and create the database.

### `Bind for 0.0.0.0:5432 failed: port is already allocated` (Docker)
Another Postgres is already running on port 5432. Either stop it (`docker ps` → find it → `docker stop <name>`) or start your container on port 5433 (see section 1, Option B) and update your user-secret port to `5433`.

### `dotnet ef` not found
Install the global tool: `dotnet tool install --global dotnet-ef`. Then restart your terminal so it picks up the new PATH entry.

### `An error occurred while reading the user secret file`
Your user-secret store may not be initialized. From the repo root:
```
dotnet user-secrets --project BudgetApp.API init
```
Then re-run the `set` command from step 4.

### Swagger loads but every request returns 500
Check the terminal where `dotnet run` is running — the EF/Npgsql exception will be printed there. Most often it's one of the issues above (connection, auth, missing DB).

### macOS: `psql: command not found`
Homebrew didn't link the binary. Run:
```bash
echo 'export PATH="/opt/homebrew/opt/postgresql@17/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc
```
(On Intel Macs, use `/usr/local/opt/...` instead of `/opt/homebrew/opt/...`.)

---

## Resetting everything (nuclear option)

If your local DB gets into a weird state and you just want a clean slate:

```
dotnet ef database drop --project BudgetApp.Infrastructure --startup-project BudgetApp.API --force
dotnet ef database update --project BudgetApp.Infrastructure --startup-project BudgetApp.API
```

This drops the database and re-applies all migrations.

---

## What's where (quick reference)

| Thing | Location |
|---|---|
| Connection string (placeholder) | `BudgetApp.API/appsettings.json` |
| Connection string (your real one) | User Secrets on your machine — never committed |
| EF migrations | `BudgetApp.Infrastructure/Migrations/` |
| Domain entities | `BudgetApp.Domain/Entities/` |
| API endpoints | `BudgetApp.API/Controllers/` |
| Swagger UI | `https://localhost:7057/swagger` |

# ptpbankintegrated
A simple solution that mimics a dashboard for a banking system. This dashboard is responsible for managing clients and their accounts.

<img width="1388" height="760" alt="image" src="https://github.com/user-attachments/assets/1a11ca5b-0f66-489b-98df-c385c3932932" />

## Solution Structure

```text
PTPBank.sln
src/
  PTPBank.Domain/    # Entities, enums, factories, domain base types
  PTPBank.Web/       # Razor Pages UI + EF Core + services
Database/
  CreateSchema.sql   # SQL schema
```

## Public Navigation

Top navigation includes only:
- **Home** (`/`)
- **Persons** (`/Persons`)
- **About** (`/About`)
- **Contact** (`/Contact`)

## Informational Pages

- **Home**: landing page with hero section, system overview, and "Get Started" action to Person Management.
- **About**: platform purpose, features, architecture, and technology stack.
- **Contact**: fictitious support details.

## Database Setup

1. Create a SQL Server database named `MSSQLLocalDB` or any name of your choice. Create a DB called `PTPBankDb`.
2. Update connection string in `src/PTPBank.Web/appsettings.json`.
3. Run EF Core migration (recommended) or execute `Database/CreateSchema.sql`.

## Local Run

```bash
cd <repo-root>

dotnet restore
dotnet build PTPBank.sln
dotnet run --project src/PTPBank.Web/PTPBank.Web.csproj
```

Then open the URL printed by ASP.NET Core (typically `http://localhost:xxxx`).

# TodoApp

Minimal ASP.NET Core todo app backed by PostgreSQL.

## Setup

Set your PostgreSQL connection string:

```bash
export DATABASE_URL="Host=localhost;Port=5432;Database=todos;Username=postgres;Password=postgres"
```

Run the app:

```bash
dotnet run
```

Open [http://localhost:5000](http://localhost:5000) (or the URL shown in the terminal).

The database schema is created automatically on startup.

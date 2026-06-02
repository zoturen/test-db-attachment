using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Set the DATABASE_URL environment variable.");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/todos", async (TodoDbContext db) =>
    await db.Todos
        .AsNoTracking()
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync());

app.MapPost("/api/todos", async (CreateTodoRequest request, TodoDbContext db) =>
{
    var title = request.Title?.Trim();
    if (string.IsNullOrWhiteSpace(title))
        return Results.BadRequest(new { error = "Title is required." });

    var todo = new TodoItem { Title = title };
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPatch("/api/todos/{id:int}", async (int id, UpdateTodoRequest request, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
        return Results.NotFound();

    if (request.IsComplete.HasValue)
        todo.IsComplete = request.IsComplete.Value;

    if (request.Title is not null)
    {
        var title = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(title))
            return Results.BadRequest(new { error = "Title cannot be empty." });
        todo.Title = title;
    }

    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/api/todos/{id:int}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
        return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

record CreateTodoRequest(string? Title);
record UpdateTodoRequest(string? Title, bool? IsComplete);

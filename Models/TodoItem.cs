namespace TodoApp.Models;

public class TodoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

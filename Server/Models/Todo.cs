namespace TodoApi.Models;

public enum TodoStatus
{
    Created,
    InProgress,
    Done
}
public class Todo
{
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign Key
    public required string Title { get; set; }
    public string? Description { get; set; } // Nullable
    public TodoStatus Status { get; set; } = TodoStatus.Created;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property to provide access to foreign key object.
    public User User { get; set; } = null!;
}
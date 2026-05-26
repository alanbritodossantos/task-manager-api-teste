using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public TaskItem(string title, string? description, DateTime? dueDate, TaskItemStatus status)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public TaskItemStatus Status { get; private set; }

    public void Update(string title, string? description, DateTime? dueDate, TaskItemStatus status)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
    }
}

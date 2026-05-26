using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public class CreateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pendente;
}

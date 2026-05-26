using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public class TaskFilterRequest
{
    public TaskItemStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
}

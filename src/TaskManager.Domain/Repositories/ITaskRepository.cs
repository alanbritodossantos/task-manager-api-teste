using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Repositories;

public interface ITaskRepository
{
    Task AddAsync(TaskItem taskItem);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(TaskItemStatus? status, DateTime? dueDate);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task UpdateAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
}

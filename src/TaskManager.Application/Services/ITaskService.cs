using TaskManager.Application.DTOs;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request);
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(TaskFilterRequest filter);
    Task<TaskResponse> GetByIdAsync(Guid id);
    Task UpdateAsync(Guid id, UpdateTaskRequest request);
    Task DeleteAsync(Guid id);
}

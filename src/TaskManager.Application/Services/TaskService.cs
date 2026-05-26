using Microsoft.Extensions.Logging;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private const int TitleMaxLength = 100;
    private const int DescriptionMaxLength = 500;

    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        ValidateTitle(request.Title);
        ValidateDescription(request.Description);
        ValidateDueDate(request.DueDate);
        ValidateStatus(request.Status);

        var taskItem = new TaskItem(
            NormalizeTitle(request.Title),
            NormalizeDescription(request.Description),
            request.DueDate?.Date,
            request.Status);

        await _taskRepository.AddAsync(taskItem);

        _logger.LogInformation("Tarefa criada: {TaskId}", taskItem.Id);

        return ToResponse(taskItem);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(TaskFilterRequest filter)
    {
        if (filter.Status.HasValue)
        {
            ValidateStatus(filter.Status.Value);
        }

        var tasks = await _taskRepository.GetAllAsync(filter.Status, filter.DueDate?.Date);
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id)
    {
        var taskItem = await GetExistingTaskAsync(id);
        return ToResponse(taskItem);
    }

    public async Task UpdateAsync(Guid id, UpdateTaskRequest request)
    {
        ValidateTitle(request.Title);
        ValidateDescription(request.Description);
        ValidateDueDate(request.DueDate);
        ValidateStatus(request.Status);

        var taskItem = await GetExistingTaskAsync(id);

        taskItem.Update(
            NormalizeTitle(request.Title),
            NormalizeDescription(request.Description),
            request.DueDate?.Date,
            request.Status);

        await _taskRepository.UpdateAsync(taskItem);

        _logger.LogInformation("Tarefa atualizada: {TaskId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var taskItem = await GetExistingTaskAsync(id);

        await _taskRepository.DeleteAsync(taskItem);

        _logger.LogInformation("Tarefa excluída: {TaskId}", id);
    }

    private async Task<TaskItem> GetExistingTaskAsync(Guid id)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            _logger.LogWarning("Tentativa de acessar tarefa inexistente: {TaskId}", id);
            throw new NotFoundException("Tarefa não encontrada");
        }

        return taskItem;
    }

    private static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessValidationException("O título é obrigatório");
        }

        if (title.Trim().Length > TitleMaxLength)
        {
            throw new BusinessValidationException($"O título deve ter no máximo {TitleMaxLength} caracteres");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description) && description.Trim().Length > DescriptionMaxLength)
        {
            throw new BusinessValidationException($"A descrição deve ter no máximo {DescriptionMaxLength} caracteres");
        }
    }

    private static void ValidateDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value.Date < DateTime.Today)
        {
            throw new BusinessValidationException("A data de vencimento não pode ser anterior à data atual");
        }
    }

    private static void ValidateStatus(TaskItemStatus status)
    {
        if (!Enum.IsDefined(typeof(TaskItemStatus), status))
        {
            throw new BusinessValidationException("Status inválido");
        }
    }

    private static string NormalizeTitle(string? title)
    {
        return title!.Trim();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static TaskResponse ToResponse(TaskItem taskItem)
    {
        return new TaskResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            DueDate = taskItem.DueDate,
            Status = taskItem.Status
        };
    }
}

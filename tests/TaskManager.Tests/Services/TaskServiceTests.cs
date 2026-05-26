using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateTask_WhenRequestIsValid()
    {
        var repository = new FakeTaskRepository();
        var service = CreateService(repository);

        var response = await service.CreateAsync(new CreateTaskRequest
        {
            Title = "Estudar API",
            Description = "Revisar endpoints",
            DueDate = DateTime.Today.AddDays(1),
            Status = TaskItemStatus.Pendente
        });

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Estudar API", response.Title);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTitleIsEmpty()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<BusinessValidationException>(() =>
            service.CreateAsync(new CreateTaskRequest
            {
                Title = " ",
                Status = TaskItemStatus.Pendente
            }));

        Assert.Equal("O título é obrigatório", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDueDateIsBeforeToday()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            service.CreateAsync(new CreateTaskRequest
            {
                Title = "Tarefa atrasada",
                DueDate = DateTime.Today.AddDays(-1),
                Status = TaskItemStatus.Pendente
            }));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        var repository = new FakeTaskRepository();
        var task = await AddTaskAsync(repository, "Tarefa cadastrada", TaskItemStatus.EmProgresso);
        var service = CreateService(repository);

        var response = await service.GetByIdAsync(task.Id);

        Assert.Equal(task.Id, response.Id);
        Assert.Equal(TaskItemStatus.EmProgresso, response.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTask_WhenTaskExists()
    {
        var repository = new FakeTaskRepository();
        var task = await AddTaskAsync(repository, "Título antigo", TaskItemStatus.Pendente);
        var service = CreateService(repository);

        await service.UpdateAsync(task.Id, new UpdateTaskRequest
        {
            Title = "Título novo",
            Description = "Descrição atualizada",
            DueDate = DateTime.Today.AddDays(2),
            Status = TaskItemStatus.Concluida
        });

        var updatedTask = repository.Items.Single(x => x.Id == task.Id);
        Assert.Equal("Título novo", updatedTask.Title);
        Assert.Equal(TaskItemStatus.Concluida, updatedTask.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(Guid.NewGuid(), new UpdateTaskRequest
            {
                Title = "Tarefa",
                Status = TaskItemStatus.Pendente
            }));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask_WhenTaskExists()
    {
        var repository = new FakeTaskRepository();
        var task = await AddTaskAsync(repository, "Tarefa para excluir", TaskItemStatus.Pendente);
        var service = CreateService(repository);

        await service.DeleteAsync(task.Id);

        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByStatus()
    {
        var repository = new FakeTaskRepository();
        await AddTaskAsync(repository, "Tarefa pendente", TaskItemStatus.Pendente);
        await AddTaskAsync(repository, "Tarefa concluída", TaskItemStatus.Concluida);
        var service = CreateService(repository);

        var tasks = await service.GetAllAsync(new TaskFilterRequest
        {
            Status = TaskItemStatus.Concluida
        });

        Assert.Single(tasks);
        Assert.Equal(TaskItemStatus.Concluida, tasks[0].Status);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByDueDate()
    {
        var repository = new FakeTaskRepository();
        var targetDate = DateTime.Today.AddDays(3);
        await AddTaskAsync(repository, "Tarefa da data", TaskItemStatus.Pendente, targetDate);
        await AddTaskAsync(repository, "Outra data", TaskItemStatus.Pendente, DateTime.Today.AddDays(5));
        var service = CreateService(repository);

        var tasks = await service.GetAllAsync(new TaskFilterRequest
        {
            DueDate = targetDate
        });

        Assert.Single(tasks);
        Assert.Equal("Tarefa da data", tasks[0].Title);
    }

    private static TaskService CreateService(FakeTaskRepository? repository = null)
    {
        return new TaskService(repository ?? new FakeTaskRepository(), NullLogger<TaskService>.Instance);
    }

    private static async Task<TaskItem> AddTaskAsync(
        FakeTaskRepository repository,
        string title,
        TaskItemStatus status,
        DateTime? dueDate = null)
    {
        var task = new TaskItem(title, null, dueDate, status);
        await repository.AddAsync(task);
        return task;
    }

    private sealed class FakeTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();

        public IReadOnlyList<TaskItem> Items => _tasks;

        public Task AddAsync(TaskItem taskItem)
        {
            _tasks.Add(taskItem);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TaskItem>> GetAllAsync(TaskItemStatus? status, DateTime? dueDate)
        {
            var query = _tasks.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (dueDate.HasValue)
            {
                var date = dueDate.Value.Date;
                query = query.Where(x => x.DueDate.HasValue && x.DueDate.Value.Date == date);
            }

            return Task.FromResult<IReadOnlyList<TaskItem>>(query.ToList());
        }

        public Task<TaskItem?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_tasks.FirstOrDefault(x => x.Id == id));
        }

        public Task UpdateAsync(TaskItem taskItem)
        {
            var index = _tasks.FindIndex(x => x.Id == taskItem.Id);

            if (index >= 0)
            {
                _tasks[index] = taskItem;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(TaskItem taskItem)
        {
            _tasks.RemoveAll(x => x.Id == taskItem.Id);
            return Task.CompletedTask;
        }
    }
}

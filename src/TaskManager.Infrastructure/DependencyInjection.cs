using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<TaskManagerDbContext>(options =>
            options.UseInMemoryDatabase("TaskManagerDb"));

        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}

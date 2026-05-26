using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data;

public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var task = modelBuilder.Entity<TaskItem>();

        task.HasKey(x => x.Id);
        task.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        task.Property(x => x.Description)
            .HasMaxLength(500);

        task.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
    }
}

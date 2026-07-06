using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Core.Entities;

namespace ProjectPilotLite.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
    public DbSet<Deliverable> Deliverables => Set<Deliverable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(project =>
        {
            project.Property(p => p.Name).IsRequired().HasMaxLength(200);
            project.Property(p => p.Description).HasMaxLength(1000);
            project.Property(p => p.Owner).IsRequired().HasMaxLength(120);
            project.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            project.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            project.HasMany(p => p.Deliverables)
                .WithOne(d => d.Project)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectTask>(task =>
        {
            task.Property(t => t.Title).IsRequired().HasMaxLength(200);
            task.Property(t => t.Description).HasMaxLength(1000);
            task.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
            task.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            task.HasIndex(t => t.ProjectId);
        });

        modelBuilder.Entity<Deliverable>(deliverable =>
        {
            deliverable.Property(d => d.Name).IsRequired().HasMaxLength(200);
            deliverable.Property(d => d.PathOrUrl).IsRequired().HasMaxLength(500);
            deliverable.Property(d => d.Comment).HasMaxLength(1000);
            deliverable.Property(d => d.Type).HasConversion<string>().HasMaxLength(30);
            deliverable.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
            deliverable.HasIndex(d => d.ProjectId);
        });
    }
}

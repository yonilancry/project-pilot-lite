using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Entities;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Todo;

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
}

using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? Deadline { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public List<ProjectTask> Tasks { get; set; } = new();
    public List<Deliverable> Deliverables { get; set; } = new();
}

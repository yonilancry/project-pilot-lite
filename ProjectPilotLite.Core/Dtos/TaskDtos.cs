using System.ComponentModel.DataAnnotations;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Dtos;

public record TaskCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;

    [Required, EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; init; } = TaskPriority.Normal;

    [Required, EnumDataType(typeof(ProjectTaskStatus))]
    public ProjectTaskStatus Status { get; init; } = ProjectTaskStatus.Todo;
}

public record TaskStatusUpdateDto
{
    [Required, EnumDataType(typeof(ProjectTaskStatus))]
    public ProjectTaskStatus Status { get; init; }
}

public record TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; init; }
    public ProjectTaskStatus Status { get; init; }
    public Guid ProjectId { get; init; }
}

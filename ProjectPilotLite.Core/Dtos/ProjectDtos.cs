using System.ComponentModel.DataAnnotations;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Dtos;

public record ProjectCreateDto : IValidatableObject
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; init; } = string.Empty;

    [Required, StringLength(120)]
    public string Owner { get; init; } = string.Empty;

    public DateOnly? StartDate { get; init; }
    public DateOnly? Deadline { get; init; }
    public ProjectStatus Status { get; init; } = ProjectStatus.Planned;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate is { } start && Deadline is { } deadline && deadline < start)
        {
            yield return new ValidationResult(
                "La date limite doit être postérieure ou égale à la date de début.",
                new[] { nameof(Deadline) });
        }
    }
}

public record ProjectStatusUpdateDto
{
    [Required, EnumDataType(typeof(ProjectStatus))]
    public ProjectStatus Status { get; init; }
}

public record ProjectSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public ProjectStatus Status { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? Deadline { get; init; }
    public int TaskCount { get; init; }
    public int DeliverableCount { get; init; }
}

public record ProjectDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public DateOnly? StartDate { get; init; }
    public DateOnly? Deadline { get; init; }
    public ProjectStatus Status { get; init; }
    public IReadOnlyList<TaskDto> Tasks { get; init; } = [];
    public IReadOnlyList<DeliverableDto> Deliverables { get; init; } = [];
}

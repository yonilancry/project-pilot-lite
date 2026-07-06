using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.Presentation;

public sealed record ProjectRow(
    Guid Id,
    string Name,
    string Owner,
    string StatusLabel,
    string DeadlineText,
    int TaskCount,
    int DeliverableCount)
{
    public static ProjectRow From(ProjectSummaryDto dto) => new(
        dto.Id,
        dto.Name,
        dto.Owner,
        EnumLabels.Label(dto.Status),
        dto.Deadline?.ToString("dd/MM/yyyy") ?? "—",
        dto.TaskCount,
        dto.DeliverableCount);
}

public sealed record TaskRow(
    Guid Id,
    string Title,
    string Description,
    string PriorityLabel,
    string StatusLabel,
    ProjectTaskStatus Status)
{
    public static TaskRow From(TaskDto dto) => new(
        dto.Id,
        dto.Title,
        dto.Description,
        EnumLabels.Label(dto.Priority),
        EnumLabels.Label(dto.Status),
        dto.Status);
}

public sealed record DeliverableRow(
    Guid Id,
    string Name,
    string TypeLabel,
    string PathOrUrl,
    string StatusLabel,
    string? Comment,
    DeliverableStatus Status)
{
    public static DeliverableRow From(DeliverableDto dto) => new(
        dto.Id,
        dto.Name,
        EnumLabels.Label(dto.Type),
        dto.PathOrUrl,
        EnumLabels.Label(dto.Status),
        dto.Comment,
        dto.Status);
}

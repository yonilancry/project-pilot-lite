using System.ComponentModel.DataAnnotations;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Dtos;

public record DeliverableCreateDto
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required, EnumDataType(typeof(DeliverableType))]
    public DeliverableType Type { get; init; } = DeliverableType.Other;

    [Required, StringLength(500)]
    public string PathOrUrl { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Comment { get; init; }
}

public record DeliverableStatusUpdateDto
{
    [Required, EnumDataType(typeof(DeliverableStatus))]
    public DeliverableStatus Status { get; init; }

    [StringLength(1000)]
    public string? Comment { get; init; }
}

public record DeliverableDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DeliverableType Type { get; init; }
    public string PathOrUrl { get; init; } = string.Empty;
    public DeliverableStatus Status { get; init; }
    public string? Comment { get; init; }
    public Guid ProjectId { get; init; }
}

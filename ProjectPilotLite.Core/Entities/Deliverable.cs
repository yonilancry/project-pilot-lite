using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Core.Entities;

public class Deliverable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeliverableType Type { get; set; } = DeliverableType.Other;
    public string PathOrUrl { get; set; } = string.Empty;
    public DeliverableStatus Status { get; set; } = DeliverableStatus.Submitted;
    public string? Comment { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
}

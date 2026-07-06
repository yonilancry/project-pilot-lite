using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Entities;

namespace ProjectPilotLite.Api.Mapping;

public static class DtoMappings
{
    public static Project ToEntity(this ProjectCreateDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        Owner = dto.Owner,
        StartDate = dto.StartDate,
        Deadline = dto.Deadline,
        Status = dto.Status
    };

    public static ProjectTask ToEntity(this TaskCreateDto dto, Guid projectId) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        Priority = dto.Priority,
        Status = dto.Status,
        ProjectId = projectId
    };

    public static Deliverable ToEntity(this DeliverableCreateDto dto, Guid projectId) => new()
    {
        Name = dto.Name,
        Type = dto.Type,
        PathOrUrl = dto.PathOrUrl,
        Comment = dto.Comment,
        ProjectId = projectId
    };

    public static TaskDto ToDto(this ProjectTask task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        ProjectId = task.ProjectId
    };

    public static DeliverableDto ToDto(this Deliverable deliverable) => new()
    {
        Id = deliverable.Id,
        Name = deliverable.Name,
        Type = deliverable.Type,
        PathOrUrl = deliverable.PathOrUrl,
        Status = deliverable.Status,
        Comment = deliverable.Comment,
        ProjectId = deliverable.ProjectId
    };

    public static ProjectDetailDto ToDetailDto(this Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        Owner = project.Owner,
        StartDate = project.StartDate,
        Deadline = project.Deadline,
        Status = project.Status,
        Tasks = project.Tasks.OrderBy(t => t.Title).Select(t => t.ToDto()).ToList(),
        Deliverables = project.Deliverables.OrderBy(d => d.Name).Select(d => d.ToDto()).ToList()
    };
}

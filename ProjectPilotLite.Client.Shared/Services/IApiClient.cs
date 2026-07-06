using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.Services;

public interface IApiClient
{
    Task<IReadOnlyList<ProjectSummaryDto>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProjectDetailDto?> GetProjectAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectDetailDto> CreateProjectAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateProjectStatusAsync(Guid id, ProjectStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskDto>> GetTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateTaskAsync(Guid projectId, TaskCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateTaskStatusAsync(Guid taskId, ProjectTaskStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliverableDto>> GetDeliverablesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<DeliverableDto> CreateDeliverableAsync(Guid projectId, DeliverableCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateDeliverableStatusAsync(Guid deliverableId, DeliverableStatus status, string? comment, CancellationToken cancellationToken = default);

    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}

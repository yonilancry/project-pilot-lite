namespace ProjectPilotLite.Core.Dtos;

public record DashboardDto
{
    public int TotalProjects { get; init; }
    public int ProjectsInProgress { get; init; }
    public int ProjectsBlocked { get; init; }
    public int TotalTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int SubmittedDeliverables { get; init; }
}

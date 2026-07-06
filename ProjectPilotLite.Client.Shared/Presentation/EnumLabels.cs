using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.Presentation;

public sealed record EnumOption<T>(T Value, string Label) where T : struct, Enum;

public static class EnumLabels
{
    public static string Label(ProjectStatus status) => status switch
    {
        ProjectStatus.Planned => "Prévu",
        ProjectStatus.InProgress => "En cours",
        ProjectStatus.Done => "Terminé",
        ProjectStatus.Blocked => "Bloqué",
        _ => status.ToString()
    };

    public static string Label(TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "Basse",
        TaskPriority.Normal => "Normale",
        TaskPriority.High => "Haute",
        _ => priority.ToString()
    };

    public static string Label(ProjectTaskStatus status) => status switch
    {
        ProjectTaskStatus.Todo => "À faire",
        ProjectTaskStatus.InProgress => "En cours",
        ProjectTaskStatus.Done => "Terminé",
        _ => status.ToString()
    };

    public static string Label(DeliverableType type) => type switch
    {
        DeliverableType.Code => "Code",
        DeliverableType.Documentation => "Documentation",
        DeliverableType.Presentation => "Présentation",
        DeliverableType.Other => "Autre",
        _ => type.ToString()
    };

    public static string Label(DeliverableStatus status) => status switch
    {
        DeliverableStatus.Submitted => "Déposé",
        DeliverableStatus.Approved => "Validé",
        DeliverableStatus.Rejected => "Refusé",
        _ => status.ToString()
    };

    public static IReadOnlyList<EnumOption<ProjectStatus>> ProjectStatuses { get; } =
        BuildOptions<ProjectStatus>(Label);

    public static IReadOnlyList<EnumOption<TaskPriority>> TaskPriorities { get; } =
        BuildOptions<TaskPriority>(Label);

    public static IReadOnlyList<EnumOption<ProjectTaskStatus>> TaskStatuses { get; } =
        BuildOptions<ProjectTaskStatus>(Label);

    public static IReadOnlyList<EnumOption<DeliverableType>> DeliverableTypes { get; } =
        BuildOptions<DeliverableType>(Label);

    private static IReadOnlyList<EnumOption<T>> BuildOptions<T>(Func<T, string> label) where T : struct, Enum
        => Enum.GetValues<T>().Select(value => new EnumOption<T>(value, label(value))).ToList();
}

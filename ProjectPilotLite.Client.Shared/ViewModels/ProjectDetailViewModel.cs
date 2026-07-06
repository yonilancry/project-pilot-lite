using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Presentation;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class ProjectDetailViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private readonly Guid _projectId;

    private ProjectDetailDto? _project;
    private EnumOption<ProjectStatus>? _selectedProjectStatus;
    private TaskRow? _selectedTask;
    private EnumOption<ProjectTaskStatus>? _selectedTaskStatus;
    private DeliverableRow? _selectedDeliverable;

    public ProjectDetailViewModel(
        IApiClient api,
        Guid projectId,
        Func<Task> back,
        Func<Guid, Task> addTask,
        Func<Guid, Task> addDeliverable)
    {
        _api = api;
        _projectId = projectId;

        BackCommand = new AsyncRelayCommand(back);
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        AddTaskCommand = new AsyncRelayCommand(() => addTask(_projectId));
        AddDeliverableCommand = new AsyncRelayCommand(() => addDeliverable(_projectId));
        ChangeProjectStatusCommand = new AsyncRelayCommand(ChangeProjectStatusAsync, () => SelectedProjectStatus is not null);
        ChangeTaskStatusCommand = new AsyncRelayCommand(ChangeTaskStatusAsync, () => SelectedTask is not null && SelectedTaskStatus is not null);
        ApproveDeliverableCommand = new AsyncRelayCommand(() => ChangeDeliverableStatusAsync(DeliverableStatus.Approved), () => SelectedDeliverable is not null);
        RejectDeliverableCommand = new AsyncRelayCommand(() => ChangeDeliverableStatusAsync(DeliverableStatus.Rejected), () => SelectedDeliverable is not null);
    }

    public ObservableCollection<TaskRow> Tasks { get; } = new();
    public ObservableCollection<DeliverableRow> Deliverables { get; } = new();

    public string ProjectName => _project?.Name ?? string.Empty;
    public string Owner => _project?.Owner ?? string.Empty;
    public string Description => _project?.Description ?? string.Empty;
    public string StatusLabel => _project is null ? string.Empty : EnumLabels.Label(_project.Status);
    public string DatesText => _project is null
        ? string.Empty
        : $"{Format(_project.StartDate)} → {Format(_project.Deadline)}";

    public IReadOnlyList<EnumOption<ProjectStatus>> ProjectStatusOptions => EnumLabels.ProjectStatuses;
    public IReadOnlyList<EnumOption<ProjectTaskStatus>> TaskStatusOptions => EnumLabels.TaskStatuses;

    public EnumOption<ProjectStatus>? SelectedProjectStatus
    {
        get => _selectedProjectStatus;
        set
        {
            if (SetProperty(ref _selectedProjectStatus, value))
            {
                ChangeProjectStatusCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public TaskRow? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value))
            {
                SelectedTaskStatus = value is null
                    ? null
                    : TaskStatusOptions.First(option => option.Value == value.Status);
                ChangeTaskStatusCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public EnumOption<ProjectTaskStatus>? SelectedTaskStatus
    {
        get => _selectedTaskStatus;
        set
        {
            if (SetProperty(ref _selectedTaskStatus, value))
            {
                ChangeTaskStatusCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public DeliverableRow? SelectedDeliverable
    {
        get => _selectedDeliverable;
        set
        {
            if (SetProperty(ref _selectedDeliverable, value))
            {
                ApproveDeliverableCommand.RaiseCanExecuteChanged();
                RejectDeliverableCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand BackCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand AddDeliverableCommand { get; }
    public AsyncRelayCommand ChangeProjectStatusCommand { get; }
    public AsyncRelayCommand ChangeTaskStatusCommand { get; }
    public AsyncRelayCommand ApproveDeliverableCommand { get; }
    public AsyncRelayCommand RejectDeliverableCommand { get; }

    public Task LoadAsync() => RunAsync(ReloadAsync);

    private async Task ReloadAsync()
    {
        var detail = await _api.GetProjectAsync(_projectId);
        if (detail is null)
        {
            ErrorMessage = "Projet introuvable.";
            return;
        }

        _project = detail;
        SelectedProjectStatus = ProjectStatusOptions.First(option => option.Value == detail.Status);

        Tasks.Clear();
        foreach (var task in detail.Tasks)
        {
            Tasks.Add(TaskRow.From(task));
        }

        Deliverables.Clear();
        foreach (var deliverable in detail.Deliverables)
        {
            Deliverables.Add(DeliverableRow.From(deliverable));
        }

        OnPropertyChanged(nameof(ProjectName));
        OnPropertyChanged(nameof(Owner));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(StatusLabel));
        OnPropertyChanged(nameof(DatesText));
    }

    private Task ChangeProjectStatusAsync() => RunAsync(async () =>
    {
        if (SelectedProjectStatus is null)
        {
            return;
        }

        await _api.UpdateProjectStatusAsync(_projectId, SelectedProjectStatus.Value);
        await ReloadAsync();
    });

    private Task ChangeTaskStatusAsync() => RunAsync(async () =>
    {
        if (SelectedTask is null || SelectedTaskStatus is null)
        {
            return;
        }

        await _api.UpdateTaskStatusAsync(SelectedTask.Id, SelectedTaskStatus.Value);
        await ReloadAsync();
    });

    private Task ChangeDeliverableStatusAsync(DeliverableStatus status) => RunAsync(async () =>
    {
        if (SelectedDeliverable is null)
        {
            return;
        }

        await _api.UpdateDeliverableStatusAsync(SelectedDeliverable.Id, status, SelectedDeliverable.Comment);
        await ReloadAsync();
    });

    private static string Format(DateOnly? date) => date?.ToString("dd/MM/yyyy") ?? "—";
}

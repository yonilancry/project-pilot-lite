using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Presentation;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class TaskCreateViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private readonly Guid _projectId;
    private readonly Func<Task> _onDone;

    private string _title = string.Empty;
    private string _description = string.Empty;
    private EnumOption<TaskPriority> _selectedPriority =
        EnumLabels.TaskPriorities.First(option => option.Value == TaskPriority.Normal);
    private EnumOption<ProjectTaskStatus> _selectedStatus =
        EnumLabels.TaskStatuses.First(option => option.Value == ProjectTaskStatus.Todo);

    public TaskCreateViewModel(IApiClient api, Guid projectId, Func<Task> onDone)
    {
        _api = api;
        _projectId = projectId;
        _onDone = onDone;
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Title));
        CancelCommand = new AsyncRelayCommand(onDone);
    }

    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public IReadOnlyList<EnumOption<TaskPriority>> PriorityOptions => EnumLabels.TaskPriorities;
    public IReadOnlyList<EnumOption<ProjectTaskStatus>> StatusOptions => EnumLabels.TaskStatuses;

    public EnumOption<TaskPriority> SelectedPriority
    {
        get => _selectedPriority;
        set => SetProperty(ref _selectedPriority, value);
    }

    public EnumOption<ProjectTaskStatus> SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    public AsyncRelayCommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private Task SaveAsync() => RunAsync(async () =>
    {
        var dto = new TaskCreateDto
        {
            Title = Title.Trim(),
            Description = Description.Trim(),
            Priority = SelectedPriority.Value,
            Status = SelectedStatus.Value
        };

        await _api.CreateTaskAsync(_projectId, dto);
        await _onDone();
    });
}

using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Presentation;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class ProjectCreateViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private readonly Func<Task> _onSaved;

    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _owner = string.Empty;
    private DateTimeOffset? _startDate;
    private DateTimeOffset? _deadline;
    private EnumOption<ProjectStatus> _selectedStatus = EnumLabels.ProjectStatuses[0];

    public ProjectCreateViewModel(IApiClient api, Func<Task> onSaved, Func<Task> onCancel)
    {
        _api = api;
        _onSaved = onSaved;
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new AsyncRelayCommand(onCancel);
    }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
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

    public string Owner
    {
        get => _owner;
        set
        {
            if (SetProperty(ref _owner, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public DateTimeOffset? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTimeOffset? Deadline
    {
        get => _deadline;
        set => SetProperty(ref _deadline, value);
    }

    public IReadOnlyList<EnumOption<ProjectStatus>> StatusOptions => EnumLabels.ProjectStatuses;

    public EnumOption<ProjectStatus> SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    public AsyncRelayCommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Owner);

    private Task SaveAsync() => RunAsync(async () =>
    {
        var dto = new ProjectCreateDto
        {
            Name = Name.Trim(),
            Description = Description.Trim(),
            Owner = Owner.Trim(),
            StartDate = ToDateOnly(StartDate),
            Deadline = ToDateOnly(Deadline),
            Status = SelectedStatus.Value
        };

        await _api.CreateProjectAsync(dto);
        await _onSaved();
    });

    private static DateOnly? ToDateOnly(DateTimeOffset? value)
        => value is { } date ? DateOnly.FromDateTime(date.Date) : null;
}

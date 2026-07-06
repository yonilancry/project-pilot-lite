using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Presentation;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class DeliverableCreateViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private readonly Guid _projectId;
    private readonly Func<Task> _onDone;

    private string _name = string.Empty;
    private string _pathOrUrl = string.Empty;
    private string _comment = string.Empty;
    private EnumOption<DeliverableType> _selectedType =
        EnumLabels.DeliverableTypes.First(option => option.Value == DeliverableType.Other);

    public DeliverableCreateViewModel(IApiClient api, Guid projectId, Func<Task> onDone)
    {
        _api = api;
        _projectId = projectId;
        _onDone = onDone;
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new AsyncRelayCommand(onDone);
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

    public string PathOrUrl
    {
        get => _pathOrUrl;
        set
        {
            if (SetProperty(ref _pathOrUrl, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Comment
    {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    public IReadOnlyList<EnumOption<DeliverableType>> TypeOptions => EnumLabels.DeliverableTypes;

    public EnumOption<DeliverableType> SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    public AsyncRelayCommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(PathOrUrl);

    private Task SaveAsync() => RunAsync(async () =>
    {
        var dto = new DeliverableCreateDto
        {
            Name = Name.Trim(),
            Type = SelectedType.Value,
            PathOrUrl = PathOrUrl.Trim(),
            Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim()
        };

        await _api.CreateDeliverableAsync(_projectId, dto);
        await _onDone();
    });
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Presentation;
using ProjectPilotLite.Client.Shared.Services;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class ProjectListViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private readonly Func<Guid, Task> _openProject;
    private ProjectRow? _selectedProject;

    public ProjectListViewModel(IApiClient api, Func<Guid, Task> openProject, Func<Task> createProject)
    {
        _api = api;
        _openProject = openProject;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        CreateCommand = new AsyncRelayCommand(createProject);
        OpenCommand = new AsyncRelayCommand(OpenSelectedAsync, () => SelectedProject is not null);
    }

    public ObservableCollection<ProjectRow> Projects { get; } = new();

    public ProjectRow? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SetProperty(ref _selectedProject, value))
            {
                OpenCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand CreateCommand { get; }
    public AsyncRelayCommand OpenCommand { get; }

    public Task LoadAsync() => RunAsync(async () =>
    {
        var projects = await _api.GetProjectsAsync();
        Projects.Clear();
        foreach (var project in projects)
        {
            Projects.Add(ProjectRow.From(project));
        }
    });

    private async Task OpenSelectedAsync()
    {
        if (SelectedProject is { } row)
        {
            await _openProject(row.Id);
        }
    }
}

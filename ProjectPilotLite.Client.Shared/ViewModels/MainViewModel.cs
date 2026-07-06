using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Services;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IApiClient _api;
    private ObservableObject? _current;

    public MainViewModel(IApiClient api)
    {
        _api = api;
        ShowProjectsCommand = new AsyncRelayCommand(ShowProjectsAsync);
        ShowDashboardCommand = new AsyncRelayCommand(ShowDashboardAsync);
        _ = ShowProjectsAsync();
    }

    public ObservableObject? Current
    {
        get => _current;
        private set => SetProperty(ref _current, value);
    }

    public ICommand ShowProjectsCommand { get; }
    public ICommand ShowDashboardCommand { get; }

    private async Task ShowProjectsAsync()
    {
        var viewModel = new ProjectListViewModel(_api, ShowProjectDetailAsync, ShowCreateProjectAsync);
        Current = viewModel;
        await viewModel.LoadAsync();
    }

    private async Task ShowDashboardAsync()
    {
        var viewModel = new DashboardViewModel(_api);
        Current = viewModel;
        await viewModel.LoadAsync();
    }

    private async Task ShowProjectDetailAsync(Guid projectId)
    {
        var viewModel = new ProjectDetailViewModel(
            _api,
            projectId,
            ShowProjectsAsync,
            ShowCreateTaskAsync,
            ShowCreateDeliverableAsync);
        Current = viewModel;
        await viewModel.LoadAsync();
    }

    private Task ShowCreateProjectAsync()
    {
        Current = new ProjectCreateViewModel(_api, ShowProjectsAsync, ShowProjectsAsync);
        return Task.CompletedTask;
    }

    private Task ShowCreateTaskAsync(Guid projectId)
    {
        Current = new TaskCreateViewModel(_api, projectId, () => ShowProjectDetailAsync(projectId));
        return Task.CompletedTask;
    }

    private Task ShowCreateDeliverableAsync(Guid projectId)
    {
        Current = new DeliverableCreateViewModel(_api, projectId, () => ShowProjectDetailAsync(projectId));
        return Task.CompletedTask;
    }
}

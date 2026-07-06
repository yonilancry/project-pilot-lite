using System.Windows.Input;
using ProjectPilotLite.Client.Shared.Mvvm;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Core.Dtos;

namespace ProjectPilotLite.Client.Shared.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IApiClient _api;
    private DashboardDto _stats = new();

    public DashboardViewModel(IApiClient api)
    {
        _api = api;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public DashboardDto Stats
    {
        get => _stats;
        private set => SetProperty(ref _stats, value);
    }

    public ICommand RefreshCommand { get; }

    public Task LoadAsync() => RunAsync(async () => Stats = await _api.GetDashboardAsync());
}

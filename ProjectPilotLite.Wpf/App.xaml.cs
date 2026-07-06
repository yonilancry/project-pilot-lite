using System;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Configuration;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Client.Shared.ViewModels;

namespace ProjectPilotLite.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5123";
        if (!baseUrl.EndsWith('/'))
        {
            baseUrl += "/";
        }

        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        var apiClient = new ApiClient(httpClient);

        var window = new MainWindow
        {
            DataContext = new MainViewModel(apiClient)
        };
        window.Show();
    }
}

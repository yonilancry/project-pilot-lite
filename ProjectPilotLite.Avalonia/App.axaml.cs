using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using ProjectPilotLite.Client.Shared.Services;
using ProjectPilotLite.Client.Shared.ViewModels;

namespace ProjectPilotLite.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(apiClient)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

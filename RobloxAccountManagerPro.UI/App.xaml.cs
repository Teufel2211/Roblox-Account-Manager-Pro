using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Services;
using RobloxAccountManagerPro.Services.Logging;
using RobloxAccountManagerPro.Services.Process;
using RobloxAccountManagerPro.Services.Security;
using RobloxAccountManagerPro.Services.Supabase;
using RobloxAccountManagerPro.UI.ViewModels;
using RobloxAccountManagerPro.UI.Views;

namespace RobloxAccountManagerPro.UI;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var supabaseUrl = ConfigurationManager.AppSettings["Supabase:Url"];
        var supabaseKey = ConfigurationManager.AppSettings["Supabase:ApiKey"];

        if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(supabaseKey))
        {
            MessageBox.Show("Supabase configuration is required. Please set Supabase:Url and Supabase:ApiKey in App.config.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var supabase = serviceProvider.GetRequiredService<ISupabaseService>();

        try
        {
            await supabase.InitializeAsync(supabaseUrl, supabaseKey);
            if (!await supabase.IsConnectedAsync())
            {
                MessageBox.Show("Unable to connect to Supabase. The application requires an active Supabase connection.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Supabase initialization failed: {ex.Message}", "Supabase Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        var mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = mainWindowViewModel;

        await mainWindowViewModel.InitializeAsync();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IProcessManagerService, ProcessManagerService>();
        services.AddHttpClient<ISupabaseService, SupabaseService>();
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<AccountManagerViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }
}

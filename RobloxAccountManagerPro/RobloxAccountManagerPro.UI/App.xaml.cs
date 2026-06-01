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
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IProcessManagerService, ProcessManagerService>();
        services.AddHttpClient<ISupabaseService, SupabaseService>();
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<Data.LocalCacheManager>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<AccountManagerViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }
}

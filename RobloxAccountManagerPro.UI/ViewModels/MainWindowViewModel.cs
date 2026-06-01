namespace RobloxAccountManagerPro.UI.ViewModels;

using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.UI.Infrastructure;

/// <summary>
/// View model for main application window coordination.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly IAccountService _accountService;
    private readonly IProcessManagerService _processManager;
    private readonly ILoggingService _logger;

    private string _currentTheme = "Dark";
    private bool _isOnline;

    public DashboardViewModel DashboardViewModel { get; }
    public AccountManagerViewModel AccountManagerViewModel { get; }

    public string CurrentTheme { get => _currentTheme; set => SetProperty(ref _currentTheme, value); }
    public bool IsOnline { get => _isOnline; set => SetProperty(ref _isOnline, value); }

    public MainWindowViewModel(
        IAccountService accountService,
        IProcessManagerService processManager,
        ILoggingService logger)
    {
        _accountService = accountService;
        _processManager = processManager;
        _logger = logger;

        DashboardViewModel = new DashboardViewModel(accountService, processManager, logger);
        AccountManagerViewModel = new AccountManagerViewModel(accountService, logger);

        _logger.LogInfo("Application started", "MainWindow");
    }

    public async Task InitializeAsync()
    {
        await DashboardViewModel.RefreshDashboardAsync();
        await AccountManagerViewModel.LoadAccountsAsync();
    }
}

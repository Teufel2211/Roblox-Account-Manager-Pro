namespace RobloxAccountManagerPro.UI.ViewModels;

using System.Windows.Input;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.UI.Infrastructure;

/// <summary>
/// View model for main application window coordination.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly IAccountService _accountService;
    private readonly IProcessManagerService _processManager;
    private readonly IEncryptionService _encryptionService;
    private readonly ILoggingService _logger;

    private string _currentTheme = "Dark";
    private bool _isOnline;
    private int _selectedTabIndex;

    public DashboardViewModel DashboardViewModel { get; }
    public AccountManagerViewModel AccountManagerViewModel { get; }

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowAccountsCommand { get; }
    public ICommand ShowSettingsCommand { get; }
    public ICommand ShowLogsCommand { get; }

    public string CurrentTheme { get => _currentTheme; set => SetProperty(ref _currentTheme, value); }
    public bool IsOnline { get => _isOnline; set => SetProperty(ref _isOnline, value); }
    public int SelectedTabIndex { get => _selectedTabIndex; set => SetProperty(ref _selectedTabIndex, value); }

    public MainWindowViewModel(
        IAccountService accountService,
        IProcessManagerService processManager,
        IEncryptionService encryptionService,
        ILoggingService logger)
    {
        _accountService = accountService;
        _processManager = processManager;
        _encryptionService = encryptionService;
        _logger = logger;

        DashboardViewModel = new DashboardViewModel(accountService, processManager, logger);
        AccountManagerViewModel = new AccountManagerViewModel(accountService, encryptionService, logger);

        ShowDashboardCommand = new RelayCommand(_ => SelectedTabIndex = 0);
        ShowAccountsCommand = new RelayCommand(_ => SelectedTabIndex = 1);
        ShowSettingsCommand = new RelayCommand(_ => SelectedTabIndex = 2);
        ShowLogsCommand = new RelayCommand(_ => SelectedTabIndex = 3);

        _logger.LogInfo("Application started", "MainWindow");
    }

    public async Task InitializeAsync()
    {
        await DashboardViewModel.RefreshDashboardAsync();
        await AccountManagerViewModel.LoadAccountsAsync();
    }
}

namespace RobloxAccountManagerPro.UI.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;
using RobloxAccountManagerPro.UI.Infrastructure;

/// <summary>
/// View model for dashboard with statistics and quick actions.
/// </summary>
public class DashboardViewModel : ViewModelBase
{
    private readonly IAccountService _accountService;
    private readonly IProcessManagerService _processManager;
    private readonly ILoggingService _logger;

    private int _totalAccounts;
    private int _favoriteAccounts;
    private int _activeInstances;
    private long _totalMemoryUsageMb;
    private bool _isLoading;

    public ObservableCollection<RobloxAccount> RecentAccounts { get; } = [];
    public ObservableCollection<ProcessInstance> ActiveProcessInstances { get; } = [];

    public ICommand StartAllCommand { get; }
    public ICommand StartFavoritesCommand { get; }
    public ICommand StopAllCommand { get; }
    public ICommand RefreshCommand { get; }

    public int TotalAccounts { get => _totalAccounts; set => SetProperty(ref _totalAccounts, value); }
    public int FavoriteAccounts { get => _favoriteAccounts; set => SetProperty(ref _favoriteAccounts, value); }
    public int ActiveInstances { get => _activeInstances; set => SetProperty(ref _activeInstances, value); }
    public long TotalMemoryUsageMb { get => _totalMemoryUsageMb; set => SetProperty(ref _totalMemoryUsageMb, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DashboardViewModel(IAccountService accountService, IProcessManagerService processManager, ILoggingService logger)
    {
        _accountService = accountService;
        _processManager = processManager;
        _logger = logger;

        StartAllCommand = new AsyncRelayCommand(_ => StartAllInstancesAsync());
        StartFavoritesCommand = new AsyncRelayCommand(_ => StartFavoritesAsync());
        StopAllCommand = new AsyncRelayCommand(_ => StopAllInstancesAsync());
        RefreshCommand = new AsyncRelayCommand(_ => RefreshDashboardAsync());
    }

    public async Task RefreshDashboardAsync()
    {
        IsLoading = true;
        try
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            var favorites = await _accountService.GetFavoriteAccountsAsync();
            var instances = await _processManager.GetActiveInstancesAsync();

            TotalAccounts = accounts.Count();
            FavoriteAccounts = favorites.Count();
            ActiveInstances = instances.Count();
            TotalMemoryUsageMb = instances.Sum(i => i.MemoryUsageMb);

            RecentAccounts.Clear();
            foreach (var account in accounts.OrderByDescending(a => a.LastUsed).Take(5))
                RecentAccounts.Add(account);

            ActiveProcessInstances.Clear();
            foreach (var instance in instances)
                ActiveProcessInstances.Add(instance);

            _logger.LogInfo("Dashboard refreshed", "Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to refresh dashboard", ex, "Dashboard");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task StartAllInstancesAsync()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        foreach (var account in accounts)
        {
            try
            {
                await _processManager.LaunchRobloxAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to launch {account.Username}", ex);
            }
        }
        await RefreshDashboardAsync();
    }

    private async Task StartFavoritesAsync()
    {
        var favorites = await _accountService.GetFavoriteAccountsAsync();
        foreach (var account in favorites)
        {
            try
            {
                await _processManager.LaunchRobloxAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to launch {account.Username}", ex);
            }
        }
        await RefreshDashboardAsync();
    }

    private async Task StopAllInstancesAsync()
    {
        await _processManager.TerminateAllInstancesAsync();
        await RefreshDashboardAsync();
    }
}

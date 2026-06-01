namespace RobloxAccountManagerPro.UI.ViewModels;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RobloxAccountManagerPro.Core.DTOs;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;
using RobloxAccountManagerPro.UI.Infrastructure;

/// <summary>
/// View model for account management with CRUD operations.
/// </summary>
public class AccountManagerViewModel : ViewModelBase
{
    private readonly IAccountService _accountService;
    private readonly ILoggingService _logger;

    private string _searchQuery = string.Empty;
    private RobloxAccount? _selectedAccount;
    private bool _isLoading;

    public ObservableCollection<RobloxAccount> Accounts { get; } = [];
    public ObservableCollection<string> Categories { get; } = [];

    public ICommand AddAccountCommand { get; }
    public ICommand EditAccountCommand { get; }
    public ICommand DeleteAccountCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }

    public string SearchQuery { get => _searchQuery; set => SetProperty(ref _searchQuery, value); }
    public RobloxAccount? SelectedAccount { get => _selectedAccount; set => SetProperty(ref _selectedAccount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public AccountManagerViewModel(IAccountService accountService, ILoggingService logger)
    {
        _accountService = accountService;
        _logger = logger;

        InitializeCategories();

        AddAccountCommand = new RelayCommand(_ => OpenAddDialog());
        EditAccountCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedAccount != null);
        DeleteAccountCommand = new AsyncRelayCommand(_ => DeleteSelectedAsync(), _ => SelectedAccount != null);
        SearchCommand = new AsyncRelayCommand(_ => SearchAsync());
        RefreshCommand = new AsyncRelayCommand(_ => LoadAccountsAsync());
        ToggleFavoriteCommand = new AsyncRelayCommand(_ => ToggleFavoriteAsync(), _ => SelectedAccount != null);
    }

    public async Task LoadAccountsAsync()
    {
        IsLoading = true;
        try
        {
            Accounts.Clear();
            var accounts = await _accountService.GetAllAccountsAsync();
            foreach (var account in accounts.OrderByDescending(a => a.IsFavorite).ThenBy(a => a.Username))
                Accounts.Add(account);
            _logger.LogInfo("Accounts loaded", "AccountManager");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load accounts", ex, "AccountManager");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadAccountsAsync();
            return;
        }

        Accounts.Clear();
        var results = await _accountService.SearchAccountsAsync(SearchQuery);
        foreach (var account in results)
            Accounts.Add(account);
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedAccount == null) return;

        var confirmed = MessageBox.Show(
            $"Are you sure you want to delete {SelectedAccount.Username}?",
            "Confirm Delete",
            MessageBoxButton.YesNo);

        if (confirmed == MessageBoxResult.Yes)
        {
            await _accountService.DeleteAccountAsync(SelectedAccount.Id);
            await LoadAccountsAsync();
        }
    }

    private async Task ToggleFavoriteAsync()
    {
        if (SelectedAccount == null) return;
        SelectedAccount.IsFavorite = !SelectedAccount.IsFavorite;
        await _accountService.UpdateAccountAsync(SelectedAccount);
        await LoadAccountsAsync();
    }

    private void OpenAddDialog()
    {
        _logger.LogInfo("Open add account dialog", "AccountManager");
    }

    private void OpenEditDialog()
    {
        _logger.LogInfo("Open edit account dialog", "AccountManager");
    }

    private void InitializeCategories()
    {
        Categories.Add("Main");
        Categories.Add("Alt");
        Categories.Add("Dev");
        Categories.Add("VIP");
    }
}

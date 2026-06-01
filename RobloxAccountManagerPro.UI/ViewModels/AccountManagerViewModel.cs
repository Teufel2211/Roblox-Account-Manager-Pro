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
    private readonly IEncryptionService _encryptionService;
    private readonly ILoggingService _logger;

    private string _searchQuery = string.Empty;
    private string _newAccountUsername = string.Empty;
    private string _newAccountPassword = string.Empty;
    private string _newAccountDisplayName = string.Empty;
    private string _newAccountCategory = string.Empty;
    private string _newAccountNotes = string.Empty;
    private bool _newAccountIsFavorite;
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
    public string NewAccountUsername { get => _newAccountUsername; set => SetProperty(ref _newAccountUsername, value); }
    public string NewAccountPassword { get => _newAccountPassword; set => SetProperty(ref _newAccountPassword, value); }
    public string NewAccountDisplayName { get => _newAccountDisplayName; set => SetProperty(ref _newAccountDisplayName, value); }
    public string NewAccountCategory { get => _newAccountCategory; set => SetProperty(ref _newAccountCategory, value); }
    public string NewAccountNotes { get => _newAccountNotes; set => SetProperty(ref _newAccountNotes, value); }
    public bool NewAccountIsFavorite { get => _newAccountIsFavorite; set => SetProperty(ref _newAccountIsFavorite, value); }
    public RobloxAccount? SelectedAccount { get => _selectedAccount; set => SetProperty(ref _selectedAccount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public AccountManagerViewModel(IAccountService accountService, IEncryptionService encryptionService, ILoggingService logger)
    {
        _accountService = accountService;
        _encryptionService = encryptionService;
        _logger = logger;

        InitializeCategories();

        AddAccountCommand = new AsyncRelayCommand(_ => AddAccountAsync());
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

    private async Task AddAccountAsync()
    {
        if (string.IsNullOrWhiteSpace(NewAccountUsername) || string.IsNullOrWhiteSpace(NewAccountPassword) || string.IsNullOrWhiteSpace(NewAccountDisplayName) || string.IsNullOrWhiteSpace(NewAccountCategory))
        {
            MessageBox.Show("Please provide username, password, display name, and category before logging in.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsLoading = true;
        try
        {
            var account = new RobloxAccount
            {
                Username = NewAccountUsername,
                DisplayName = NewAccountDisplayName,
                Category = NewAccountCategory,
                Notes = NewAccountNotes,
                IsFavorite = NewAccountIsFavorite,
                EncryptedPassword = _encryptionService.EncryptPassword(NewAccountPassword),
                CreatedAt = DateTime.UtcNow,
                LastUsed = DateTime.UtcNow,
                IsActive = true
            };

            await _accountService.CreateAccountAsync(account);
            await LoadAccountsAsync();
            ClearNewAccountForm();
            MessageBox.Show("Account successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add account", ex, "AccountManager");
            MessageBox.Show($"Unable to add account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearNewAccountForm()
    {
        NewAccountUsername = string.Empty;
        NewAccountPassword = string.Empty;
        NewAccountDisplayName = string.Empty;
        NewAccountCategory = string.Empty;
        NewAccountNotes = string.Empty;
        NewAccountIsFavorite = false;
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

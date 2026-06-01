namespace RobloxAccountManagerPro.Services;

using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Account management service combining Supabase and local operations.
/// </summary>
public class AccountService : IAccountService
{
    private readonly ISupabaseService _supabaseService;
    private readonly ILoggingService _logger;

    public AccountService(ISupabaseService supabaseService, ILoggingService logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    public async Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync()
    {
        try
        {
            return await _supabaseService.GetAllAccountsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to fetch accounts", ex, "AccountService");
            throw;
        }
    }

    public async Task<RobloxAccount?> GetAccountByIdAsync(Guid id)
    {
        return await _supabaseService.GetAccountAsync(id);
    }

    public async Task<RobloxAccount?> GetAccountByUsernameAsync(string username)
    {
        var accounts = await GetAllAccountsAsync();
        return accounts.FirstOrDefault(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<RobloxAccount> CreateAccountAsync(RobloxAccount account)
    {
        var result = await _supabaseService.InsertAccountAsync(account);
        _logger.LogInfo($"Created account: {account.Username}", "AccountService");
        return result;
    }

    public async Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account)
    {
        var result = await _supabaseService.UpdateAccountAsync(account);
        _logger.LogInfo($"Updated account: {account.Username}", "AccountService");
        return result;
    }

    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        var result = await _supabaseService.DeleteAccountAsync(id);
        _logger.LogInfo($"Deleted account: {id}", "AccountService");
        return result;
    }

    public async Task<IEnumerable<RobloxAccount>> SearchAccountsAsync(string query)
    {
        var accounts = await GetAllAccountsAsync();
        var lower = query.ToLower();
        return accounts.Where(a =>
            a.Username.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
            a.DisplayName.Contains(lower, StringComparison.OrdinalIgnoreCase) ||
            a.Notes?.Contains(lower, StringComparison.OrdinalIgnoreCase) == true ||
            a.Category.Contains(lower, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<IEnumerable<RobloxAccount>> GetAccountsByTagAsync(string tag)
    {
        var accounts = await GetAllAccountsAsync();
        return accounts.Where(a => a.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<RobloxAccount>> GetFavoriteAccountsAsync()
    {
        var accounts = await GetAllAccountsAsync();
        return accounts.Where(a => a.IsFavorite);
    }
}

namespace RobloxAccountManagerPro.Core.Interfaces;

using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Interface for Supabase database operations.
/// </summary>
public interface ISupabaseService
{
    Task InitializeAsync(string url, string key);
    Task<bool> IsConnectedAsync();
    
    // Accounts
    Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync();
    Task<RobloxAccount?> GetAccountAsync(Guid id);
    Task<RobloxAccount> InsertAccountAsync(RobloxAccount account);
    Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account);
    Task<bool> DeleteAccountAsync(Guid id);
    
    // Activity Logs
    Task<ActivityLog> InsertActivityLogAsync(ActivityLog log);
    Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(Guid accountId, int limit = 50);
    
    // Settings
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> UpsertSettingsAsync(AppSettings settings);
}

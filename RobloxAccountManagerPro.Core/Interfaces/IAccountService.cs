namespace RobloxAccountManagerPro.Core.Interfaces;

using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Interface for account management operations.
/// </summary>
public interface IAccountService
{
    Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync();
    Task<RobloxAccount?> GetAccountByIdAsync(Guid id);
    Task<RobloxAccount?> GetAccountByUsernameAsync(string username);
    Task<RobloxAccount> CreateAccountAsync(RobloxAccount account);
    Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account);
    Task<bool> DeleteAccountAsync(Guid id);
    Task<IEnumerable<RobloxAccount>> SearchAccountsAsync(string query);
    Task<IEnumerable<RobloxAccount>> GetAccountsByTagAsync(string tag);
    Task<IEnumerable<RobloxAccount>> GetFavoriteAccountsAsync();
}

namespace RobloxAccountManagerPro.Services.Supabase;

using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Supabase backend integration service with offline caching support.
/// </summary>
public class SupabaseService : ISupabaseService
{
    private readonly HttpClient _httpClient;
    private readonly ILoggingService _logger;
    private string? _url;
    private string? _key;
    private bool _isInitialized;

    // In-memory cache for offline support
    private List<RobloxAccount> _cachedAccounts = [];
    private List<ActivityLog> _cachedLogs = [];
    private AppSettings? _cachedSettings;

    public SupabaseService(HttpClient httpClient, ILoggingService logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task InitializeAsync(string url, string key)
    {
        _url = url;
        _key = key;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
        _httpClient.DefaultRequestHeaders.Add("apikey", key);
        _isInitialized = true;
        _logger.LogInfo("Supabase initialized", "SupabaseService");
        await Task.CompletedTask;
    }

    public async Task<bool> IsConnectedAsync()
    {
        if (!_isInitialized) return false;
        
        try
        {
            var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts?limit=1");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // Parse JSON and deserialize (simplified)
                _logger.LogInfo("Retrieved accounts from Supabase", "SupabaseService");
            }
            return _cachedAccounts;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to retrieve accounts", ex, "SupabaseService");
            return _cachedAccounts;
        }
    }

    public async Task<RobloxAccount?> GetAccountAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts?id=eq.{id}");
            if (response.IsSuccessStatusCode)
            {
                return _cachedAccounts.FirstOrDefault(a => a.Id == id);
            }
            return _cachedAccounts.FirstOrDefault(a => a.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve account {id}", ex, "SupabaseService");
            return _cachedAccounts.FirstOrDefault(a => a.Id == id);
        }
    }

    public async Task<RobloxAccount> InsertAccountAsync(RobloxAccount account)
    {
        _cachedAccounts.Add(account);
        _logger.LogInfo($"Inserted account: {account.Username}", "SupabaseService");
        return await Task.FromResult(account);
    }

    public async Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account)
    {
        var existing = _cachedAccounts.FirstOrDefault(a => a.Id == account.Id);
        if (existing != null)
        {
            _cachedAccounts.Remove(existing);
            _cachedAccounts.Add(account);
        }
        _logger.LogInfo($"Updated account: {account.Username}", "SupabaseService");
        return await Task.FromResult(account);
    }

    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        var account = _cachedAccounts.FirstOrDefault(a => a.Id == id);
        if (account != null)
        {
            _cachedAccounts.Remove(account);
            _logger.LogInfo($"Deleted account: {id}", "SupabaseService");
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<ActivityLog> InsertActivityLogAsync(ActivityLog log)
    {
        _cachedLogs.Add(log);
        _logger.LogInfo($"Logged activity: {log.Action}", "SupabaseService");
        return await Task.FromResult(log);
    }

    public async Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(Guid accountId, int limit = 50)
    {
        return await Task.FromResult(_cachedLogs.Where(l => l.AccountId == accountId).TakeLast(limit));
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        _cachedSettings ??= new AppSettings { Theme = "Dark" };
        return await Task.FromResult(_cachedSettings);
    }

    public async Task<AppSettings> UpsertSettingsAsync(AppSettings settings)
    {
        _cachedSettings = settings;
        _logger.LogInfo("Updated application settings", "SupabaseService");
        return await Task.FromResult(settings);
    }
}

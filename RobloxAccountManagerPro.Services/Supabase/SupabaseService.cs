namespace RobloxAccountManagerPro.Services.Supabase;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Supabase backend integration service that always operates through Supabase.
/// Local data caching is intentionally disabled so offline operations fail.
/// </summary>
public class SupabaseService : ISupabaseService
{
    private readonly HttpClient _httpClient;
    private readonly ILoggingService _logger;
    private string? _url;
    private string? _key;
    private bool _isInitialized;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SupabaseService(HttpClient httpClient, ILoggingService logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task InitializeAsync(string url, string key)
    {
        _url = url?.TrimEnd('/');
        _key = key;

        if (string.IsNullOrWhiteSpace(_url) || string.IsNullOrWhiteSpace(_key))
            throw new InvalidOperationException("Supabase URL and API key are required.");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _key);
        _httpClient.DefaultRequestHeaders.Add("apikey", _key);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _isInitialized = true;
        _logger.LogInfo("Supabase initialized", "SupabaseService");
        await Task.CompletedTask;
    }

    public async Task<bool> IsConnectedAsync()
    {
        if (!_isInitialized) return false;

        try
        {
            var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts?select=*&limit=1");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Supabase service has not been initialized.");
    }

    public async Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync()
    {
        EnsureInitialized();

        var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts?select=*");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"GetAllAccountsAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to retrieve accounts from Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var accounts = JsonSerializer.Deserialize<List<RobloxAccount>>(json, _jsonOptions);
        _logger.LogInfo("Retrieved accounts from Supabase", "SupabaseService");
        return accounts ?? new List<RobloxAccount>();
    }

    public async Task<RobloxAccount?> GetAccountAsync(Guid id)
    {
        EnsureInitialized();

        var response = await _httpClient.GetAsync($"{_url}/rest/v1/accounts?select=*&id=eq.{id}");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"GetAccountAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException($"Failed to retrieve account {id} from Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var accounts = JsonSerializer.Deserialize<List<RobloxAccount>>(json, _jsonOptions);
        return accounts?.FirstOrDefault();
    }

    public async Task<RobloxAccount> InsertAccountAsync(RobloxAccount account)
    {
        EnsureInitialized();

        var response = await _httpClient.PostAsJsonAsync($"{_url}/rest/v1/accounts?return=representation", account);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"InsertAccountAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to insert account into Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var accounts = JsonSerializer.Deserialize<List<RobloxAccount>>(json, _jsonOptions);
        var result = accounts?.FirstOrDefault() ?? account;
        _logger.LogInfo($"Inserted account: {result.Username}", "SupabaseService");
        return result;
    }

    public async Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account)
    {
        EnsureInitialized();

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_url}/rest/v1/accounts?id=eq.{account.Id}&return=representation")
        {
            Content = JsonContent.Create(account)
        };

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"UpdateAccountAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to update account in Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var accounts = JsonSerializer.Deserialize<List<RobloxAccount>>(json, _jsonOptions);
        var result = accounts?.FirstOrDefault() ?? account;
        _logger.LogInfo($"Updated account: {result.Username}", "SupabaseService");
        return result;
    }

    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        EnsureInitialized();

        var response = await _httpClient.DeleteAsync($"{_url}/rest/v1/accounts?id=eq.{id}");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"DeleteAccountAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to delete account from Supabase.");
        }

        _logger.LogInfo($"Deleted account: {id}", "SupabaseService");
        return true;
    }

    public async Task<ActivityLog> InsertActivityLogAsync(ActivityLog log)
    {
        EnsureInitialized();

        var response = await _httpClient.PostAsJsonAsync($"{_url}/rest/v1/activity_logs?return=representation", log);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"InsertActivityLogAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to insert activity log into Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var logs = JsonSerializer.Deserialize<List<ActivityLog>>(json, _jsonOptions);
        var result = logs?.FirstOrDefault() ?? log;
        _logger.LogInfo($"Logged activity: {result.Action}", "SupabaseService");
        return result;
    }

    public async Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(Guid accountId, int limit = 50)
    {
        EnsureInitialized();

        var response = await _httpClient.GetAsync($"{_url}/rest/v1/activity_logs?accountId=eq.{accountId}&select=*&limit={limit}");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"GetActivityLogsAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to retrieve activity logs from Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var logs = JsonSerializer.Deserialize<List<ActivityLog>>(json, _jsonOptions);
        return logs ?? new List<ActivityLog>();
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        EnsureInitialized();

        var response = await _httpClient.GetAsync($"{_url}/rest/v1/app_settings?select=*&limit=1");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"GetSettingsAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to retrieve settings from Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var settings = JsonSerializer.Deserialize<List<AppSettings>>(json, _jsonOptions);
        return settings?.FirstOrDefault() ?? new AppSettings { Theme = "Dark" };
    }

    public async Task<AppSettings> UpsertSettingsAsync(AppSettings settings)
    {
        EnsureInitialized();

        var response = await _httpClient.PostAsJsonAsync($"{_url}/rest/v1/app_settings?return=representation", settings);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"UpsertSettingsAsync failed: {response.StatusCode} {content}", null, "SupabaseService");
            throw new HttpRequestException("Failed to update settings in Supabase.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var settingsList = JsonSerializer.Deserialize<List<AppSettings>>(json, _jsonOptions);
        var result = settingsList?.FirstOrDefault() ?? settings;
        _logger.LogInfo("Updated application settings", "SupabaseService");
        return result;
    }
}

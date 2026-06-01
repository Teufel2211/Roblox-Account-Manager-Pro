namespace RobloxAccountManagerPro.Data;

using System.Text.Json;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Local cache manager for offline support with JSON serialization.
/// </summary>
public class LocalCacheManager
{
    private readonly string _cacheDirectory;
    private readonly ILoggingService _logger;
    private const string AccountsFile = "accounts.json";
    private const string SettingsFile = "settings.json";

    public LocalCacheManager(ILoggingService logger)
    {
        _logger = logger;
        _cacheDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RobloxAMP", "Cache");
        
        Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task SaveAccountsAsync(IEnumerable<RobloxAccount> accounts)
    {
        try
        {
            var path = Path.Combine(_cacheDirectory, AccountsFile);
            var json = JsonSerializer.Serialize(accounts.ToList(), new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
            _logger.LogInfo("Accounts cached successfully", "LocalCache");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to cache accounts", ex, "LocalCache");
        }
    }

    public async Task<List<RobloxAccount>> LoadAccountsAsync()
    {
        try
        {
            var path = Path.Combine(_cacheDirectory, AccountsFile);
            if (!File.Exists(path)) return [];

            var json = await File.ReadAllTextAsync(path);
            var accounts = JsonSerializer.Deserialize<List<RobloxAccount>>(json) ?? [];
            _logger.LogInfo($"Loaded {accounts.Count} accounts from cache", "LocalCache");
            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load accounts from cache", ex, "LocalCache");
            return [];
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            var path = Path.Combine(_cacheDirectory, SettingsFile);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
            _logger.LogInfo("Settings cached successfully", "LocalCache");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to cache settings", ex, "LocalCache");
        }
    }

    public async Task<AppSettings?> LoadSettingsAsync()
    {
        try
        {
            var path = Path.Combine(_cacheDirectory, SettingsFile);
            if (!File.Exists(path)) return null;

            var json = await File.ReadAllTextAsync(path);
            var settings = JsonSerializer.Deserialize<AppSettings>(json);
            _logger.LogInfo("Settings loaded from cache", "LocalCache");
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load settings from cache", ex, "LocalCache");
            return null;
        }
    }

    public async Task ClearCacheAsync()
    {
        try
        {
            if (Directory.Exists(_cacheDirectory))
                Directory.Delete(_cacheDirectory, true);
            Directory.CreateDirectory(_cacheDirectory);
            _logger.LogInfo("Cache cleared", "LocalCache");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to clear cache", ex, "LocalCache");
        }
        await Task.CompletedTask;
    }

    public async Task ExportAsync(string targetPath)
    {
        try
        {
            var accounts = await LoadAccountsAsync();
            var settings = await LoadSettingsAsync();

            var export = new { Accounts = accounts, Settings = settings, ExportDate = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(targetPath, json);
            _logger.LogInfo($"Data exported to {targetPath}", "LocalCache");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to export data", ex, "LocalCache");
        }
    }
}

# API Documentation

## Overview

The Roblox Account Manager Pro provides a comprehensive API for account management, process control, and security operations.

---

## 📚 Service Interfaces

### IAccountService

Account management operations with CRUD functionality.

```csharp
public interface IAccountService
{
    /// <summary>
    /// Retrieves all stored Roblox accounts.
    /// </summary>
    Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync();

    /// <summary>
    /// Retrieves a specific account by ID.
    /// </summary>
    Task<RobloxAccount?> GetAccountByIdAsync(Guid id);

    /// <summary>
    /// Finds an account by username.
    /// </summary>
    Task<RobloxAccount?> GetAccountByUsernameAsync(string username);

    /// <summary>
    /// Creates a new account.
    /// </summary>
    Task<RobloxAccount> CreateAccountAsync(RobloxAccount account);

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account);

    /// <summary>
    /// Deletes an account.
    /// </summary>
    Task<bool> DeleteAccountAsync(Guid id);

    /// <summary>
    /// Searches accounts by username, display name, or notes.
    /// </summary>
    Task<IEnumerable<RobloxAccount>> SearchAccountsAsync(string query);

    /// <summary>
    /// Retrieves accounts with specific tag.
    /// </summary>
    Task<IEnumerable<RobloxAccount>> GetAccountsByTagAsync(string tag);

    /// <summary>
    /// Gets all favorite accounts.
    /// </summary>
    Task<IEnumerable<RobloxAccount>> GetFavoriteAccountsAsync();
}
```

### IProcessManagerService

Roblox process launching and monitoring.

```csharp
public interface IProcessManagerService
{
    /// <summary>
    /// Launches a Roblox instance for the specified account.
    /// </summary>
    Task<ProcessInstance> LaunchRobloxAsync(RobloxAccount account);

    /// <summary>
    /// Gets all active Roblox process instances.
    /// </summary>
    Task<IEnumerable<ProcessInstance>> GetActiveInstancesAsync();

    /// <summary>
    /// Retrieves specific instance by process ID.
    /// </summary>
    Task<ProcessInstance?> GetInstanceByProcessIdAsync(int pid);

    /// <summary>
    /// Terminates a specific process instance.
    /// </summary>
    Task<bool> TerminateInstanceAsync(int pid);

    /// <summary>
    /// Terminates all active instances.
    /// </summary>
    Task<bool> TerminateAllInstancesAsync();

    /// <summary>
    /// Refreshes memory and CPU metrics for all instances.
    /// </summary>
    Task RefreshProcessMetricsAsync();

    /// <summary>
    /// Fired when a new process is started.
    /// </summary>
    event EventHandler<ProcessInstance>? ProcessStarted;

    /// <summary>
    /// Fired when a process is terminated.
    /// </summary>
    event EventHandler<ProcessInstance>? ProcessTerminated;
}
```

### IEncryptionService

Encryption and security operations.

```csharp
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a password using AES-256.
    /// </summary>
    string EncryptPassword(string plaintext);

    /// <summary>
    /// Decrypts an AES-256 encrypted password.
    /// </summary>
    string DecryptPassword(string encrypted);

    /// <summary>
    /// Creates a PBKDF2-SHA256 hash of a password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash.
    /// </summary>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Generates a cryptographically secure random token.
    /// </summary>
    string GenerateSecureToken(int length = 32);

    /// <summary>
    /// Securely wipes sensitive data from memory.
    /// </summary>
    void SecureWipeMemory(byte[] data);
}
```

### ISupabaseService

Cloud database integration.

```csharp
public interface ISupabaseService
{
    /// <summary>
    /// Initializes Supabase connection.
    /// </summary>
    Task InitializeAsync(string url, string key);

    /// <summary>
    /// Checks if Supabase connection is active.
    /// </summary>
    Task<bool> IsConnectedAsync();

    // Account operations
    Task<IEnumerable<RobloxAccount>> GetAllAccountsAsync();
    Task<RobloxAccount?> GetAccountAsync(Guid id);
    Task<RobloxAccount> InsertAccountAsync(RobloxAccount account);
    Task<RobloxAccount> UpdateAccountAsync(RobloxAccount account);
    Task<bool> DeleteAccountAsync(Guid id);

    // Activity logging
    Task<ActivityLog> InsertActivityLogAsync(ActivityLog log);
    Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(Guid accountId, int limit = 50);

    // Settings
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> UpsertSettingsAsync(AppSettings settings);
}
```

### ILoggingService

Application logging and diagnostics.

```csharp
public interface ILoggingService
{
    void LogInfo(string message, string? category = null);
    void LogWarning(string message, string? category = null);
    void LogError(string message, Exception? exception = null, string? category = null);
    void LogDebug(string message, string? category = null);
    void ClearLogs();
    List<string> GetRecentLogs(int count = 50);
}
```

---

## 🔧 Usage Examples

### Adding an Account

```csharp
var accountService = serviceProvider.GetRequiredService<IAccountService>();

var newAccount = new RobloxAccount
{
    Username = "MyAccount",
    DisplayName = "My Main Account",
    Category = "Main",
    Tags = new List<string> { "VIP", "Main" },
    Notes = "Primary gaming account"
};

var created = await accountService.CreateAccountAsync(newAccount);
Console.WriteLine($"Created account: {created.Id}");
```

### Launching Multiple Instances

```csharp
var processManager = serviceProvider.GetRequiredService<IProcessManagerService>();

var accounts = await accountService.GetFavoriteAccountsAsync();

foreach (var account in accounts)
{
    var instance = await processManager.LaunchRobloxAsync(account);
    logger.LogInfo($"Launched {account.Username} (PID: {instance.ProcessId})");
}
```

### Monitoring Processes

```csharp
processManager.ProcessStarted += (sender, instance) =>
{
    logger.LogInfo($"Process started: {instance.Username}");
};

processManager.ProcessTerminated += (sender, instance) =>
{
    logger.LogInfo($"Process terminated: {instance.Username}");
};

await processManager.RefreshProcessMetricsAsync();
var active = await processManager.GetActiveInstancesAsync();

foreach (var instance in active)
{
    Console.WriteLine($"{instance.Username}: {instance.MemoryUsageMb}MB");
}
```

### Encrypting Passwords

```csharp
var encryption = serviceProvider.GetRequiredService<IEncryptionService>();

var password = "MySecurePassword123!";
var encrypted = encryption.EncryptPassword(password);
var decrypted = encryption.DecryptPassword(encrypted);

Console.WriteLine($"Original: {password}");
Console.WriteLine($"Encrypted: {encrypted}");
Console.WriteLine($"Decrypted: {decrypted}");
```

### Searching Accounts

```csharp
var results = await accountService.SearchAccountsAsync("Main");

foreach (var account in results)
{
    Console.WriteLine($"{account.Username} ({account.Category})");
}
```

---

## 📊 Models

### RobloxAccount

```csharp
public class RobloxAccount
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? Notes { get; set; }
    public required string Category { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public List<string> Tags { get; set; }
    public DateTime? LastUsed { get; set; }
    public string? EncryptedPassword { get; set; }
    public bool IsActive { get; set; }
    public int LaunchCount { get; set; }
}
```

### ProcessInstance

```csharp
public class ProcessInstance
{
    public int ProcessId { get; set; }
    public Guid AccountId { get; set; }
    public required string Username { get; set; }
    public DateTime StartTime { get; set; }
    public ProcessStatus Status { get; set; }
    public long MemoryUsageMb { get; set; }
    public double CpuUsagePercent { get; set; }
}

public enum ProcessStatus
{
    Running,
    Suspended,
    Closed,
    Error
}
```

### ActivityLog

```csharp
public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public required string Action { get; set; }
    public required string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public int? ProcessId { get; set; }
    public long? MemoryUsageMb { get; set; }
}
```

---

## 🔐 Error Handling

All async operations return `Task<T>` or `Task` and may throw:

- `InvalidOperationException`: Operation failed or invalid state
- `ArgumentNullException`: Required parameter is null
- `IOException`: Disk/file operations failed
- `HttpRequestException`: Network/Supabase operations failed

**Always use try-catch:**

```csharp
try
{
    await accountService.DeleteAccountAsync(accountId);
}
catch (Exception ex)
{
    logger.LogError($"Delete failed: {ex.Message}", ex);
}
```

---

## 🚀 Performance Tips

1. **Batch Operations**: Retrieve all accounts once, filter locally
2. **Caching**: Services cache results, use refresh methods to reload
3. **Async**: All long operations are async, never block UI thread
4. **Monitoring**: Process metrics update every 1000ms (configurable)

---

## 📝 Version History

- **2.0.0**: Initial API release

---

**Last Updated:** January 2024

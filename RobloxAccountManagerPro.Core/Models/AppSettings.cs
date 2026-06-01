namespace RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Represents application settings including security and theme preferences.
/// </summary>
public class AppSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? MasterPasswordHash { get; set; }
    public bool AutoLockEnabled { get; set; } = true;
    public int AutoLockMinutes { get; set; } = 15;
    public required string Theme { get; set; } = "Dark";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
    public bool WindowsHelloEnabled { get; set; }
    public bool OnlineBackupEnabled { get; set; } = true;
    public string? SupabaseUrl { get; set; }
    public string? SupabaseKey { get; set; }
}

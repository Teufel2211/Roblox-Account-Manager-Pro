namespace RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Represents a Roblox account with metadata and tracking information.
/// </summary>
public class RobloxAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? Notes { get; set; }
    public required string Category { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTime? LastUsed { get; set; }
    public string? EncryptedPassword { get; set; }
    public bool IsActive { get; set; } = true;
    public int LaunchCount { get; set; }
}

namespace RobloxAccountManagerPro.Core.DTOs;

/// <summary>
/// Data Transfer Object for account creation requests.
/// </summary>
public class CreateAccountRequest
{
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? Notes { get; set; }
    public required string Category { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? Password { get; set; }
}

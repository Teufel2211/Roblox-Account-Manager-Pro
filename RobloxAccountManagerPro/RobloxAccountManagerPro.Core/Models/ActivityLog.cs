namespace RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Represents an activity log entry for audit and tracking purposes.
/// </summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public required string Action { get; set; }
    public required string Status { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
    public int? ProcessId { get; set; }
    public long? MemoryUsageMb { get; set; }
}

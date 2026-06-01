namespace RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Represents a running Roblox process instance.
/// </summary>
public class ProcessInstance
{
    public int ProcessId { get; set; }
    public Guid AccountId { get; set; }
    public required string Username { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public ProcessStatus Status { get; set; } = ProcessStatus.Running;
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

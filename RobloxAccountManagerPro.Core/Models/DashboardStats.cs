namespace RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Represents dashboard statistics and metrics.
/// </summary>
public class DashboardStats
{
    public int TotalAccounts { get; set; }
    public int FavoriteAccounts { get; set; }
    public int ActiveInstances { get; set; }
    public long TotalMemoryUsageMb { get; set; }
    public DateTime LastSyncTime { get; set; }
    public bool IsOnline { get; set; }
    public List<string> RecentActivities { get; set; } = [];
}

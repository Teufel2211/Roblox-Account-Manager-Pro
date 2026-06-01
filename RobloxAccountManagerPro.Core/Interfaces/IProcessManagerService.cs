namespace RobloxAccountManagerPro.Core.Interfaces;

using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Interface for process management and Roblox instance launching.
/// </summary>
public interface IProcessManagerService
{
    Task<ProcessInstance> LaunchRobloxAsync(RobloxAccount account);
    Task<IEnumerable<ProcessInstance>> GetActiveInstancesAsync();
    Task<ProcessInstance?> GetInstanceByProcessIdAsync(int pid);
    Task<bool> TerminateInstanceAsync(int pid);
    Task<bool> TerminateAllInstancesAsync();
    Task RefreshProcessMetricsAsync();
    event EventHandler<ProcessInstance>? ProcessStarted;
    event EventHandler<ProcessInstance>? ProcessTerminated;
}

namespace RobloxAccountManagerPro.Services.Process;

using System.Diagnostics;
using RobloxAccountManagerPro.Core.Constants;
using RobloxAccountManagerPro.Core.Interfaces;
using RobloxAccountManagerPro.Core.Models;

/// <summary>
/// Manages Roblox process instances with monitoring and lifecycle control.
/// </summary>
public class ProcessManagerService : IProcessManagerService
{
    private readonly Dictionary<int, ProcessInstance> _activeInstances = [];
    private readonly ILoggingService _logger;
    private Timer? _metricsTimer;

    public event EventHandler<ProcessInstance>? ProcessStarted;
    public event EventHandler<ProcessInstance>? ProcessTerminated;

    public ProcessManagerService(ILoggingService logger)
    {
        _logger = logger;
        InitializeMetricsMonitoring();
    }

    public async Task<ProcessInstance> LaunchRobloxAsync(RobloxAccount account)
    {
        try
        {
            var process = new ProcessStartInfo
            {
                FileName = "RobloxPlayerLauncher.exe",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            var proc = System.Diagnostics.Process.Start(process);
            if (proc == null)
                throw new InvalidOperationException("Failed to start Roblox process");

            var instance = new ProcessInstance
            {
                ProcessId = proc.Id,
                AccountId = account.Id,
                Username = account.Username,
                Status = ProcessStatus.Running
            };

            _activeInstances[proc.Id] = instance;
            _logger.LogInfo($"Launched Roblox for account: {account.Username}", "ProcessManager");
            ProcessStarted?.Invoke(this, instance);

            return await Task.FromResult(instance);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to launch Roblox for account {account.Username}", ex, "ProcessManager");
            throw;
        }
    }

    public async Task<IEnumerable<ProcessInstance>> GetActiveInstancesAsync()
    {
        CleanupClosedProcesses();
        return await Task.FromResult(_activeInstances.Values.AsEnumerable());
    }

    public async Task<ProcessInstance?> GetInstanceByProcessIdAsync(int pid)
    {
        _activeInstances.TryGetValue(pid, out var instance);
        return await Task.FromResult(instance);
    }

    public async Task<bool> TerminateInstanceAsync(int pid)
    {
        try
        {
            if (_activeInstances.TryGetValue(pid, out var instance))
            {
                var process = System.Diagnostics.Process.GetProcessById(pid);
                process.Kill();
                instance.Status = ProcessStatus.Closed;
                _activeInstances.Remove(pid);
                ProcessTerminated?.Invoke(this, instance);
                _logger.LogInfo($"Terminated process {pid}", "ProcessManager");
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to terminate process {pid}", ex, "ProcessManager");
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> TerminateAllInstancesAsync()
    {
        var pids = _activeInstances.Keys.ToList();
        foreach (var pid in pids)
            await TerminateInstanceAsync(pid);

        return await Task.FromResult(true);
    }

    public async Task RefreshProcessMetricsAsync()
    {
        foreach (var (pid, instance) in _activeInstances)
        {
            try
            {
                var process = System.Diagnostics.Process.GetProcessById(pid);
                instance.MemoryUsageMb = process.WorkingSet64 / (1024 * 1024);
                instance.Status = ProcessStatus.Running;
            }
            catch
            {
                instance.Status = ProcessStatus.Closed;
            }
        }

        await Task.CompletedTask;
    }

    private void InitializeMetricsMonitoring()
    {
        _metricsTimer = new Timer(_ => RefreshProcessMetricsAsync().FireAndForget(), 
            null, 
            TimeSpan.FromSeconds(1), 
            TimeSpan.FromMilliseconds(AppConstants.ProcessMetricsUpdateIntervalMs));
    }

    private void CleanupClosedProcesses()
    {
        var closedPids = _activeInstances
            .Where(kv => kv.Value.Status == ProcessStatus.Closed || !ProcessExists(kv.Key))
            .Select(kv => kv.Key)
            .ToList();

        foreach (var pid in closedPids)
            _activeInstances.Remove(pid);
    }

    private static bool ProcessExists(int pid)
    {
        try
        {
            System.Diagnostics.Process.GetProcessById(pid);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _metricsTimer?.Dispose();
    }
}

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        _ = task.ConfigureAwait(false);
    }
}

namespace RobloxAccountManagerPro.Core.Interfaces;

/// <summary>
/// Interface for service implementations requiring logging capabilities.
/// </summary>
public interface ILoggingService
{
    void LogInfo(string message, string? category = null);
    void LogWarning(string message, string? category = null);
    void LogError(string message, Exception? exception = null, string? category = null);
    void LogDebug(string message, string? category = null);
    void ClearLogs();
    List<string> GetRecentLogs(int count = 50);
}

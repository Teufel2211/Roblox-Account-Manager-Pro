namespace RobloxAccountManagerPro.Services.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;
using RobloxAccountManagerPro.Core.Interfaces;

/// <summary>
/// Provides in-memory logging with circular buffer storage.
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly ConcurrentQueue<LogEntry> _logs = new();
    private const int MaxLogs = 1000;

    private class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Exception { get; set; }
    }

    public void LogInfo(string message, string? category = null)
        => AddLog("INFO", message, category);

    public void LogWarning(string message, string? category = null)
        => AddLog("WARN", message, category);

    public void LogError(string message, Exception? exception = null, string? category = null)
        => AddLog("ERROR", $"{message}\n{exception?.Message}", category, exception?.StackTrace);

    public void LogDebug(string message, string? category = null)
        => AddLog("DEBUG", message, category);

    public void ClearLogs() => _logs.Clear();

    public List<string> GetRecentLogs(int count = 50)
    {
        var logs = _logs.ToList();
        return logs
            .TakeLast(count)
            .Select(l => $"[{l.Timestamp:yyyy-MM-dd HH:mm:ss}] [{l.Level}] {l.Message}")
            .ToList();
    }

    private void AddLog(string level, string message, string? category = null, string? stackTrace = null)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message,
            Category = category,
            Exception = stackTrace
        };

        _logs.Enqueue(entry);

        if (_logs.Count > MaxLogs)
            _logs.TryDequeue(out _);

        Debug.WriteLine($"[{entry.Timestamp:HH:mm:ss}] [{level}] {message}");
    }
}

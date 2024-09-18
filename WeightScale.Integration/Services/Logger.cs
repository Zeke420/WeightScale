using System;
using System.IO;

namespace WeightScale.Integration.Services
{
    public class Logger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object();

        public Logger(string logFilePath)
        {
            _logFilePath = Path.Combine(logFilePath, "log.txt");
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message, Exception ex)
        {
            Log("ERROR", $"{message} - Exception: {ex.Message}");
        }

        private void Log(string logLevel, string message)
        {
            lock (_lock)
            {
                using (var writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}");
                }
            }
        }
    }
}

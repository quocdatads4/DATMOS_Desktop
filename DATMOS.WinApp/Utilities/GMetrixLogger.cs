using DATMOS.WinApp.Grading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DATMOS.WinApp.Utilities
{
    // Logger class for detailed logging
    public class GMetrixLogger
    {
        private static readonly object _lock = new object();
        private static string _logDirectory;
        private static string _logFilePath;
        
        static GMetrixLogger()
        {
            // Initialize log directory - Use project directory as requested
            string projectDir = @"C:\Users\QuocDat-PC\Documents\GitHub\DATMOS_Desktop\DATMOS.WinApp";
            _logDirectory = Path.Combine(projectDir, "Log");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(_logDirectory);
            
            // Set log file path with current date
            _logFilePath = Path.Combine(_logDirectory, $"GMetrix_{DateTime.Now:yyyyMMdd}.log");
            
            // Also keep old log location for compatibility
            string oldLogDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DATMOS",
                "Logs"
            );
            Directory.CreateDirectory(oldLogDir);
        }
        
        public static void Log(string message, LogLevel level = LogLevel.INFO)
        {
            try
            {
                lock (_lock)
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
                    
                    // Write to file
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                    
                    // Also write to console for debugging
                    Console.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERROR] Failed to write log: {ex.Message}");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] Original message: {message}");
            }
        }
        
        public static void LogGradingStart(string projectId, string studentFilePath)
        {
            Log($"Bắt đầu chấm điểm Project {projectId}", LogLevel.INFO);
            Log($"File học viên: {studentFilePath}", LogLevel.INFO);
        }
        
        public static void LogGradingResult(string projectId, int totalScore, int maxScore, bool passed)
        {
            Log($"Kết quả chấm điểm Project {projectId}: {totalScore}/{maxScore} - {(passed ? "ĐẬU" : "RỚT")}", LogLevel.INFO);
        }
        
        public static void LogGradingDetails(string projectId, List<WordGrader.GradingItem> items)
        {
            Log($"Chi tiết chấm điểm Project {projectId}:", LogLevel.INFO);
            foreach (var item in items)
            {
                string status = item.IsCorrect ? "✓" : "✗";
                Log($"  {status} {item.Description}: {item.Score}/{item.MaxScore}", LogLevel.INFO);
                if (!string.IsNullOrEmpty(item.Feedback))
                    Log($"    Feedback: {item.Feedback}", LogLevel.INFO);
            }
        }
        
        public static void LogError(string context, Exception ex)
        {
            Log($"LỖI trong {context}: {ex.Message}", LogLevel.ERROR);
            Log($"StackTrace: {ex.StackTrace}", LogLevel.ERROR);
        }
        
        public static void LogWebMessage(string message)
        {
            Log($"Nhận message từ web: {message}", LogLevel.DEBUG);
        }
        
        public static void LogWordOperation(string operation, string projectId, bool success, string details = "")
        {
            string status = success ? "THÀNH CÔNG" : "THẤT BẠI";
            string logMessage = $"Thao tác Word - {operation} Project {projectId}: {status}";
            if (!string.IsNullOrEmpty(details))
                logMessage += $" | {details}";
            Log(logMessage, success ? LogLevel.INFO : LogLevel.WARNING);
        }
        
        public static string GetLogFilePath()
        {
            return _logFilePath;
        }
        
        public static List<string> GetRecentLogs(int maxLines = 100)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    var lines = File.ReadAllLines(_logFilePath);
                    return lines.Reverse().Take(maxLines).Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                LogError("GetRecentLogs", ex);
            }
            return new List<string>();
        }
    }
    
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
}

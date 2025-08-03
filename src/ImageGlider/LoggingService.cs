using System;
using System.Globalization;
using System.IO;

namespace ImageGlider;

/// <summary>
/// 日志服务类，提供日志记录功能
/// </summary>
public class LoggingService : IDisposable
{
    private readonly StreamWriter _logWriter;
    private readonly string _logFilePath;
    private bool _disposed = false;

    /// <summary>
    /// 初始化日志服务
    /// </summary>
    /// <param name="logDirectory">日志目录</param>
    /// <param name="logFilePrefix">日志文件前缀</param>
    public LoggingService(string logDirectory, string logFilePrefix = "ImageGlider")
    {
        // 确保日志目录存在
        Directory.CreateDirectory(logDirectory);

        // 构造日志文件名称（前缀_时间戳.log）
        var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        _logFilePath = Path.Combine(logDirectory, $"{logFilePrefix}_{timeStamp}.log");

        _logWriter = new StreamWriter(_logFilePath, append: true);
    }

    /// <summary>
    /// 记录日志信息
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="writeToConsole">是否同时输出到控制台</param>
    public void Log(string message, bool writeToConsole = true)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(LoggingService));

        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        
        if (writeToConsole)
        {
            Console.WriteLine(logMessage);
        }
        
        _logWriter.WriteLine(logMessage);
        _logWriter.Flush();
    }

    /// <summary>
    /// 记录错误信息
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="exception">异常对象</param>
    /// <param name="writeToConsole">是否同时输出到控制台</param>
    public void LogError(string message, Exception? exception = null, bool writeToConsole = true)
    {
        var errorMessage = exception != null 
            ? $"ERROR: {message} - {exception.Message}"
            : $"ERROR: {message}";
        
        Log(errorMessage, writeToConsole);
    }

    /// <summary>
    /// 记录警告信息
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <param name="writeToConsole">是否同时输出到控制台</param>
    public void LogWarning(string message, bool writeToConsole = true)
    {
        Log($"WARNING: {message}", writeToConsole);
    }

    /// <summary>
    /// 记录信息
    /// </summary>
    /// <param name="message">信息消息</param>
    /// <param name="writeToConsole">是否同时输出到控制台</param>
    public void LogInfo(string message, bool writeToConsole = true)
    {
        Log($"INFO: {message}", writeToConsole);
    }

    /// <summary>
    /// 获取日志文件路径
    /// </summary>
    public string LogFilePath => _logFilePath;

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源的具体实现
    /// </summary>
    /// <param name="disposing">是否正在释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _logWriter?.Dispose();
            _disposed = true;
        }
    }
}
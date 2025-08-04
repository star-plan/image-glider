using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;

namespace ImageGlider.Tests;

/// <summary>
/// LoggingService 的单元测试
/// </summary>
public class LoggingServiceTests
{
    /// <summary>
    /// 测试日志服务初始化 - 创建日志文件
    /// </summary>
    [Fact]
    public void LoggingService_Initialize_CreatesLogFile()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            using var logger = new LoggingService(tempDir, "Test");
            logger.Log("Test message");

            // Assert
            Assert.True(File.Exists(logger.LogFilePath));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试日志服务 - 写入文件
    /// </summary>
    [Fact]
    public void LoggingService_Log_WritesToFile()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var testMessage = "This is a test log message";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            using (var logger = new LoggingService(tempDir, "Test"))
            {
                logger.Log(testMessage, writeToConsole: false);
            } // Dispose 确保文件被刷新和关闭

            // Assert
            var logFiles = Directory.GetFiles(tempDir, "*.log");
            Assert.Single(logFiles);
            var content = File.ReadAllText(logFiles[0]);
            Assert.Contains(testMessage, content);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试日志服务 - 不同日志级别写入正确的前缀
    /// </summary>
    [Fact]
    public void LoggingService_DifferentLogLevels_WritesCorrectPrefixes()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            using (var logger = new LoggingService(tempDir, "Test"))
            {
                logger.LogInfo("Info message", writeToConsole: false);
                logger.LogWarning("Warning message", writeToConsole: false);
                logger.LogError("Error message", writeToConsole: false);
            }

            // Assert
            var logFiles = Directory.GetFiles(tempDir, "*.log");
            var content = File.ReadAllText(logFiles[0]);
            Assert.Contains("INFO: Info message", content);
            Assert.Contains("WARNING: Warning message", content);
            Assert.Contains("ERROR: Error message", content);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }
}
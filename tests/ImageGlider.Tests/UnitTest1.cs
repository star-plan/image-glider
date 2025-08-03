using System.IO;
using Xunit;
using ImageGlider;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 类的单元测试
/// </summary>
public class ImageConverterTests
{
    /// <summary>
    /// 测试单文件转换 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void ConvertImage_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.png";

        // Act
        var result = ImageConverter.ConvertImage(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试批量转换 - 目录不存在的情况
    /// </summary>
    [Fact]
    public void BatchConvert_DirectoryNotExists_ReturnsErrorResult()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = "output";
        var sourceExt = ".jfif";
        var targetExt = ".jpeg";

        // Act
        var result = ImageConverter.BatchConvert(sourceDir, outputDir, sourceExt, targetExt);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量转换 - 空目录的情况
    /// </summary>
    [Fact]
    public void BatchConvert_EmptyDirectory_ReturnsZeroFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            var result = ImageConverter.BatchConvert(tempDir, outputDir, ".jfif", ".jpeg");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.TotalFiles);
            Assert.Equal(0, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
        }
    }

    /// <summary>
    /// 测试扩展名格式化 - 自动添加点号
    /// </summary>
    [Fact]
    public void BatchConvert_ExtensionWithoutDot_AutoAddsDot()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act - 使用不带点号的扩展名
            var result = ImageConverter.BatchConvert(tempDir, outputDir, "jfif", "jpeg");

            // Assert - 应该正常处理，不会出错
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.TotalFiles); // 空目录
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
        }
    }
}

/// <summary>
/// LoggingService 类的单元测试
/// </summary>
public class LoggingServiceTests
{
    /// <summary>
    /// 测试日志服务初始化
    /// </summary>
    [Fact]
    public void LoggingService_Initialize_CreatesLogFile()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            // Act
            using var logger = new LoggingService(tempDir, "Test");
            
            // Assert
            Assert.True(Directory.Exists(tempDir));
            Assert.True(File.Exists(logger.LogFilePath));
            Assert.Contains("Test_", logger.LogFilePath);
            Assert.EndsWith(".log", logger.LogFilePath);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// 测试日志写入功能
    /// </summary>
    [Fact]
    public void LoggingService_Log_WritesToFile()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var testMessage = "Test log message";
        
        try
        {
            // Act
            using (var logger = new LoggingService(tempDir, "Test"))
            {
                logger.Log(testMessage, writeToConsole: false);
            } // Dispose 确保文件被刷新和关闭
            
            // Assert
            var logFiles = Directory.GetFiles(tempDir, "*.log");
            Assert.Single(logFiles);
            
            var logContent = File.ReadAllText(logFiles[0]);
            Assert.Contains(testMessage, logContent);
            Assert.Contains(DateTime.Now.ToString("yyyy-MM-dd"), logContent);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// 测试不同级别的日志记录
    /// </summary>
    [Fact]
    public void LoggingService_DifferentLogLevels_WritesCorrectPrefixes()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            // Act
            using (var logger = new LoggingService(tempDir, "Test"))
            {
                logger.LogInfo("Info message", writeToConsole: false);
                logger.LogWarning("Warning message", writeToConsole: false);
                logger.LogError("Error message", writeToConsole: false);
            }
            
            // Assert
            var logFiles = Directory.GetFiles(tempDir, "*.log");
            var logContent = File.ReadAllText(logFiles[0]);
            
            Assert.Contains("INFO: Info message", logContent);
            Assert.Contains("WARNING: Warning message", logContent);
            Assert.Contains("ERROR: Error message", logContent);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}
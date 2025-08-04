using System.IO;
using Xunit;
using ImageGlider;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
/// ImageConverter 尺寸调整功能的单元测试
/// </summary>
public class ImageConverterResizeTests
{
    /// <summary>
    /// 创建测试用的图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    private void CreateTestImage(string filePath, int width, int height)
    {
        using var image = new Image<Rgba32>(width, height);
        // 填充为蓝色背景
        image.Mutate(x => x.BackgroundColor(Color.Blue));
        
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        image.Save(filePath, new JpegEncoder());
    }

    /// <summary>
    /// 测试单文件尺寸调整 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void ResizeImage_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output_resized.jpg";

        // Act
        var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, 100);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试单文件尺寸调整 - 成功调整尺寸
    /// </summary>
    [Fact]
    public void ResizeImage_ValidFile_ResizesSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, 100, ResizeMode.Stretch);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证调整后的尺寸
            using var resizedImage = Image.Load(targetFile);
            Assert.Equal(100, resizedImage.Width);
            Assert.Equal(100, resizedImage.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 保持宽高比模式
    /// </summary>
    [Fact]
    public void ResizeImage_KeepAspectRatio_MaintainsProportions()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            CreateTestImage(sourceFile, 400, 200); // 2:1 宽高比

            // Act - 目标尺寸 200x200，但应保持宽高比
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 200, 200, ResizeMode.KeepAspectRatio);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证保持了宽高比（应该是 200x100）
            using var resizedImage = Image.Load(targetFile);
            Assert.Equal(200, resizedImage.Width);
            Assert.Equal(100, resizedImage.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 只指定宽度
    /// </summary>
    [Fact]
    public void ResizeImage_WidthOnly_CalculatesHeightAutomatically()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            CreateTestImage(sourceFile, 400, 200); // 2:1 宽高比

            // Act - 只指定宽度为 100
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, null, ResizeMode.KeepAspectRatio);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证高度自动计算（应该是 100x50）
            using var resizedImage = Image.Load(targetFile);
            Assert.Equal(100, resizedImage.Width);
            Assert.Equal(50, resizedImage.Height);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// 测试批量尺寸调整 - 目录不存在的情况
    /// </summary>
    [Fact]
    public void BatchResize_DirectoryNotExists_ReturnsErrorResult()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = "output";
        var sourceExt = ".jpg";

        // Act
        var result = ImageConverter.BatchResize(sourceDir, outputDir, sourceExt, 100, 100);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量尺寸调整 - 空目录的情况
    /// </summary>
    [Fact]
    public void BatchResize_EmptyDirectory_ReturnsZeroFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            var result = ImageConverter.BatchResize(tempDir, outputDir, ".jpg", 100, 100);

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
    /// 测试批量尺寸调整 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchResize_MultipleFiles_ProcessesAllFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建测试文件
            var file1 = Path.Combine(tempDir, "test1.jpg");
            var file2 = Path.Combine(tempDir, "test2.jpg");
            CreateTestImage(file1, 200, 200);
            CreateTestImage(file2, 300, 300);
            
            // Act
            var result = ImageConverter.BatchResize(tempDir, outputDir, ".jpg", 100, 100, ResizeMode.Stretch);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件（BatchResize会添加_resized后缀）
            var outputFile1 = Path.Combine(outputDir, "test1_resized.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2_resized.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
            
            // 验证尺寸
            using var resized1 = Image.Load(outputFile1);
            using var resized2 = Image.Load(outputFile2);
            Assert.Equal(100, resized1.Width);
            Assert.Equal(100, resized1.Height);
            Assert.Equal(100, resized2.Width);
            Assert.Equal(100, resized2.Height);
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
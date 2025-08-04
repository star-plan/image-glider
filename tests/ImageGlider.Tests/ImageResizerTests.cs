using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Enums;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 尺寸调整功能的单元测试
/// </summary>
public class ImageResizerTests
{
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
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, 100, ImageGlider.Enums.ResizeMode.Stretch);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 100, 100));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 保持宽高比模式
    /// </summary>
    [Fact]
    public void ResizeImage_KeepAspectRatio_MaintainsProportions()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 400, 200); // 2:1 宽高比

            // Act - 目标尺寸 200x200，但应保持宽高比
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 200, 200, ImageGlider.Enums.ResizeMode.KeepAspectRatio);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 200, 100)); // 验证保持了宽高比（应该是 200x100）
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 只指定宽度
    /// </summary>
    [Fact]
    public void ResizeImage_WidthOnly_CalculatesHeightAutomatically()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 400, 200); // 2:1 宽高比

            // Act - 只指定宽度为 100
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, null, ImageGlider.Enums.ResizeMode.KeepAspectRatio);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 100, 50)); // 验证高度自动计算（应该是 100x50）
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
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
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        
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
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试批量尺寸调整 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchResize_MultipleFiles_ProcessesAllFiles()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建测试文件
            var file1 = Path.Combine(tempDir, "test1.jpg");
            var file2 = Path.Combine(tempDir, "test2.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);
            TestImageHelper.CreateTestImage(file2, 300, 300);
            
            // Act
            var result = ImageConverter.BatchResize(tempDir, outputDir, ".jpg", 100, 100, ImageGlider.Enums.ResizeMode.Stretch);

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
            Assert.True(TestImageHelper.VerifyImageSize(outputFile1, 100, 100));
            Assert.True(TestImageHelper.VerifyImageSize(outputFile2, 100, 100));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }
}
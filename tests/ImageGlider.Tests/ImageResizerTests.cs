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
            
            // 验证输出文件存在
            var outputFile1 = Path.Combine(outputDir, "test1.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2.jpg");
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

    /// <summary>
    /// 测试尺寸调整 - 无效路径参数
    /// </summary>
    [Theory]
    [InlineData("", "output.jpg")]
    [InlineData("source.jpg", "")]
    [InlineData(null, "output.jpg")]
    [InlineData("source.jpg", null)]
    public void ResizeImage_InvalidPaths_ReturnsFalse(string sourceFile, string targetFile)
    {
        // Act
        var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, 100);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试尺寸调整 - 零或负数尺寸
    /// </summary>
    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, 0)]
    [InlineData(-10, 100)]
    [InlineData(100, -10)]
    [InlineData(0, 0)]
    [InlineData(-5, -5)]
    public void ResizeImage_InvalidDimensions_ReturnsFalse(int width, int height)
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
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, width, height);

            // Assert
            Assert.False(result);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 极大尺寸
    /// </summary>
    [Fact]
    public void ResizeImage_VeryLargeDimensions_HandlesGracefully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act - 尝试调整到非常大的尺寸
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 50000, 50000);

            // Assert
            // 根据实现，这可能成功或失败，但不应该崩溃
            if (result)
            {
                Assert.True(File.Exists(targetFile));
            }
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 无效图像文件
    /// </summary>
    [Fact]
    public void ResizeImage_InvalidImageFile_ReturnsFalse()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "invalid.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(sourceFile, "This is not an image");

            // Act
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, 100, 100);

            // Assert
            Assert.False(result);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试尺寸调整 - 只指定高度
    /// </summary>
    [Fact]
    public void ResizeImage_HeightOnly_CalculatesWidthAutomatically()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 400, 200); // 2:1 宽高比

            // Act - 只指定高度为 100
            var result = ImageConverter.ResizeImage(sourceFile, targetFile, null, 100, ImageGlider.Enums.ResizeMode.KeepAspectRatio);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 200, 100)); // 验证宽度自动计算（应该是 200x100）
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试批量尺寸调整 - 无效扩展名（应该返回0个文件）
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    public void BatchResize_InvalidExtension_ReturnsZeroFiles(string extension)
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();

        try
        {
            Directory.CreateDirectory(tempDir);

            // 创建一个jpg文件，但搜索无效扩展名
            var file1 = Path.Combine(tempDir, "test1.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);

            // Act
            var result = ImageConverter.BatchResize(tempDir, outputDir, extension, 100, 100);

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
    /// 测试批量尺寸调整 - null扩展名处理
    /// </summary>
    [Fact]
    public void BatchResize_NullExtension_HandlesGracefully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();

        try
        {
            Directory.CreateDirectory(tempDir);

            // 创建一个jpg文件
            var file1 = Path.Combine(tempDir, "test1.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);

            // Act
            var result = ImageConverter.BatchResize(tempDir, outputDir, null!, 100, 100);

            // Assert - 应该处理null扩展名而不崩溃
            Assert.NotNull(result);
            // 可能返回0个文件或有错误信息，但不应该抛出异常
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试批量尺寸调整 - 输出目录与源目录相同
    /// </summary>
    [Fact]
    public void BatchResize_SameSourceAndOutputDirectory_ProcessesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();

        try
        {
            Directory.CreateDirectory(tempDir);

            // 创建测试文件
            var file1 = Path.Combine(tempDir, "test1.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);

            // Act - 使用相同的源目录和输出目录
            var result = ImageConverter.BatchResize(tempDir, tempDir, ".jpg", 100, 100);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.TotalFiles);
            Assert.Equal(1, result.SuccessfulConversions);

            // 验证输出文件存在
            var outputFile = Path.Combine(tempDir, "test1.jpg");
            Assert.True(File.Exists(outputFile));
            Assert.True(TestImageHelper.VerifyImageSize(outputFile, 100, 100));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }
}
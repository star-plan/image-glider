using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 图像格式转换功能的单元测试
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
    /// 测试单文件转换 - 成功转换格式
    /// </summary>
    [Fact]
    public void ConvertImage_ValidFile_ConvertsSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.png");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.ConvertImage(sourceFile, targetFile);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证文件格式
            using var image = Image.Load(targetFile);
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
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
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
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

    /// <summary>
    /// 测试批量转换 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchConvert_MultipleFiles_ProcessesAllFiles()
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
            TestImageHelper.CreateTestImage(file1, 100, 100);
            TestImageHelper.CreateTestImage(file2, 150, 150);
            
            // Act
            var result = ImageConverter.BatchConvert(tempDir, outputDir, ".jpg", ".png");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件存在
            var outputFile1 = Path.Combine(outputDir, "test1.png");
            var outputFile2 = Path.Combine(outputDir, "test2.png");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
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
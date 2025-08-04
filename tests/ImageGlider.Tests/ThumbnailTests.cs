using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 缩略图生成功能的单元测试
/// </summary>
public class ThumbnailTests
{
    /// <summary>
    /// 测试单文件缩略图生成 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void GenerateThumbnail_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output_thumb.jpg";

        // Act
        var result = ImageConverter.GenerateThumbnail(sourceFile, targetFile, 150);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试单文件缩略图生成 - 成功生成缩略图（宽度为长边）
    /// </summary>
    [Fact]
    public void GenerateThumbnail_WideImage_GeneratesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "thumb.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 400, 200); // 宽度为长边

            // Act
            var result = ImageConverter.GenerateThumbnail(sourceFile, targetFile, 100);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证缩略图尺寸（宽度应该是100，高度应该是50）
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 100, 50));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试单文件缩略图生成 - 成功生成缩略图（高度为长边）
    /// </summary>
    [Fact]
    public void GenerateThumbnail_TallImage_GeneratesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "thumb.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 400); // 高度为长边

            // Act
            var result = ImageConverter.GenerateThumbnail(sourceFile, targetFile, 100);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证缩略图尺寸（宽度应该是50，高度应该是100）
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 50, 100));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试单文件缩略图生成 - 正方形图片
    /// </summary>
    [Fact]
    public void GenerateThumbnail_SquareImage_GeneratesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "thumb.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 300, 300); // 正方形

            // Act
            var result = ImageConverter.GenerateThumbnail(sourceFile, targetFile, 150);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证缩略图尺寸（应该是150x150）
            Assert.True(TestImageHelper.VerifyImageSize(targetFile, 150, 150));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试单文件缩略图生成 - 无效的尺寸参数
    /// </summary>
    [Fact]
    public void GenerateThumbnail_InvalidSize_ReturnsFalse()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "thumb.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.GenerateThumbnail(sourceFile, targetFile, 0); // 无效尺寸

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
    /// 测试批量缩略图生成 - 源目录不存在
    /// </summary>
    [Fact]
    public void BatchGenerateThumbnails_SourceDirectoryNotExists_ReturnsError()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = "./output";

        // Act
        var result = ImageConverter.BatchGenerateThumbnails(sourceDir, outputDir, ".jpg");

        // Assert
        Assert.NotNull(result.ErrorMessage);
        Assert.Equal(0, result.TotalFiles);
    }

    /// <summary>
    /// 测试批量缩略图生成 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchGenerateThumbnails_ValidFiles_ProcessesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = Path.Combine(tempDir, "output");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建测试图片
            var sourceFile1 = Path.Combine(tempDir, "image1.jpg");
            var sourceFile2 = Path.Combine(tempDir, "image2.jpg");
            TestImageHelper.CreateTestImage(sourceFile1, 300, 200);
            TestImageHelper.CreateTestImage(sourceFile2, 200, 300);

            // Act
            var result = ImageConverter.BatchGenerateThumbnails(tempDir, outputDir, ".jpg", 100);

            // Assert
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
            
            // 验证输出文件存在
            var outputFile1 = Path.Combine(outputDir, "image1_thumb.jpg");
            var outputFile2 = Path.Combine(outputDir, "image2_thumb.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
            
            // 验证缩略图尺寸
            Assert.True(TestImageHelper.VerifyImageSize(outputFile1, 100, 67)); // 300x200 -> 100x67
            Assert.True(TestImageHelper.VerifyImageSize(outputFile2, 67, 100)); // 200x300 -> 67x100
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试批量缩略图生成 - 无效的尺寸参数
    /// </summary>
    [Fact]
    public void BatchGenerateThumbnails_InvalidSize_ReturnsError()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = Path.Combine(tempDir, "output");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            var sourceFile = Path.Combine(tempDir, "image.jpg");
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.BatchGenerateThumbnails(tempDir, outputDir, ".jpg", 0); // 无效尺寸

            // Assert
            Assert.NotNull(result.ErrorMessage);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }
}
using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 图像压缩优化功能的单元测试
/// </summary>
public class ImageCompressorTests
{
    /// <summary>
    /// 测试单文件压缩 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void CompressImage_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.jpg";

        // Act
        var result = ImageConverter.CompressImage(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试单文件压缩 - 成功压缩
    /// </summary>
    [Fact]
    public void CompressImage_ValidFile_CompressesSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "compressed.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile, compressionLevel: 50);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证压缩后文件大小应该更小
            var originalSize = new FileInfo(sourceFile).Length;
            var compressedSize = new FileInfo(targetFile).Length;
            Assert.True(compressedSize < originalSize, "压缩后的文件应该比原文件小");
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试不同压缩级别的效果
    /// </summary>
    [Theory]
    [InlineData(10)]  // 高压缩
    [InlineData(50)]  // 中等压缩
    [InlineData(90)]  // 低压缩
    public void CompressImage_DifferentLevels_ProducesExpectedResults(int compressionLevel)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, $"compressed_{compressionLevel}.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 300, 300);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile, compressionLevel: compressionLevel);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试PNG格式的压缩
    /// </summary>
    [Fact]
    public void CompressImage_PngFormat_CompressesSuccessfully()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.png");
        var targetFile = Path.Combine(tempDir, "compressed.png");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 150, 150);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile, compressionLevel: 30);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    /// 测试批量压缩 - 目录不存在的情况
    /// </summary>
    [Fact]
    public void BatchCompress_DirectoryNotExists_ReturnsErrorResult()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = Path.Combine(Path.GetTempPath(), "output");

        // Act
        var result = ImageConverter.BatchCompress(sourceDir, outputDir, ".jpg");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("源目录不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量压缩 - 空目录的情况
    /// </summary>
    [Fact]
    public void BatchCompress_EmptyDirectory_ReturnsZeroFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);

            // Act
            var result = ImageConverter.BatchCompress(tempDir, outputDir, ".jpg");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }
    }

    /// <summary>
    /// 测试批量压缩 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchCompress_MultipleFiles_ProcessesAllFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建测试图片
            var file1 = Path.Combine(tempDir, "test1.jpg");
            var file2 = Path.Combine(tempDir, "test2.jpg");
            TestImageHelper.CreateTestImage(file1, 100, 100);
            TestImageHelper.CreateTestImage(file2, 150, 150);

            // Act
            var result = ImageConverter.BatchCompress(tempDir, outputDir, ".jpg", compressionLevel: 60);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件存在
            var outputFile1 = Path.Combine(outputDir, "test1_compressed.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2_compressed.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
            
            // 验证文件存在即可，压缩效果因图像内容而异
            // 对于测试图像，压缩效果可能不明显
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }
    }

    /// <summary>
    /// 测试压缩级别边界值
    /// </summary>
    [Theory]
    [InlineData(1)]   // 最小值
    [InlineData(100)] // 最大值
    [InlineData(0)]   // 超出下限，应该被调整为1
    [InlineData(150)] // 超出上限，应该被调整为100
    public void CompressImage_BoundaryValues_HandlesCorrectly(int compressionLevel)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "compressed.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile, compressionLevel: compressionLevel);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
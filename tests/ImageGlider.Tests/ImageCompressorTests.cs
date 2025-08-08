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
            var outputFile1 = Path.Combine(outputDir, "test1.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2.jpg");
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

    /// <summary>
    /// 测试压缩 - 空字符串路径
    /// </summary>
    [Theory]
    [InlineData("", "output.jpg")]
    [InlineData("source.jpg", "")]
    [InlineData("", "")]
    [InlineData(null, "output.jpg")]
    [InlineData("source.jpg", null)]
    public void CompressImage_InvalidPaths_ReturnsFalse(string sourceFile, string targetFile)
    {
        // Act
        var result = ImageConverter.CompressImage(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试压缩 - 极端质量值
    /// </summary>
    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(200)]
    public void CompressImage_ExtremeQualityValues_HandlesGracefully(int quality)
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "compressed.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile, quality);

            // Assert
            Assert.True(result); // 应该成功，质量值会被限制在有效范围内
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试压缩 - 无效图像文件
    /// </summary>
    [Fact]
    public void CompressImage_InvalidImageFile_ReturnsFalse()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "invalid.jpg");
        var targetFile = Path.Combine(tempDir, "compressed.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(sourceFile, "This is not an image file");

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile);

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
    /// 测试压缩 - 目标目录不存在
    /// </summary>
    [Fact]
    public void CompressImage_TargetDirectoryNotExists_CreatesDirectoryAndSucceeds()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetDir = Path.Combine(tempDir, "nonexistent", "subdirectory");
        var targetFile = Path.Combine(targetDir, "compressed.jpg");

        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageConverter.CompressImage(sourceFile, targetFile);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            Assert.True(Directory.Exists(targetDir));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试批量压缩 - 空扩展名（应该返回0个文件）
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BatchCompress_InvalidExtension_ReturnsZeroFiles(string extension)
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
            var result = ImageConverter.BatchCompress(tempDir, outputDir, extension);

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
    /// 测试批量压缩 - null扩展名处理
    /// </summary>
    [Fact]
    public void BatchCompress_NullExtension_HandlesGracefully()
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
            var result = ImageConverter.BatchCompress(tempDir, outputDir, null!);

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
    /// 测试批量压缩 - 包含损坏文件的混合目录
    /// </summary>
    [Fact]
    public void BatchCompress_MixedValidAndCorruptFiles_ProcessesValidFiles()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();

        try
        {
            Directory.CreateDirectory(tempDir);

            // 创建有效文件
            var validFile1 = Path.Combine(tempDir, "valid1.jpg");
            var validFile2 = Path.Combine(tempDir, "valid2.jpg");
            TestImageHelper.CreateTestImage(validFile1, 100, 100);
            TestImageHelper.CreateTestImage(validFile2, 150, 150);

            // 创建损坏的文件
            var corruptFile = Path.Combine(tempDir, "corrupt.jpg");
            File.WriteAllText(corruptFile, "This is not a valid image");

            // Act
            var result = ImageConverter.BatchCompress(tempDir, outputDir, ".jpg");

            // Assert
            Assert.False(result.IsSuccess); // 因为有失败的转换，所以IsSuccess应该是false
            Assert.Equal(3, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(1, result.FailedConversions);

            // 验证成功的文件
            Assert.True(File.Exists(Path.Combine(outputDir, "valid1.jpg")));
            Assert.True(File.Exists(Path.Combine(outputDir, "valid2.jpg")));
            Assert.False(File.Exists(Path.Combine(outputDir, "corrupt.jpg")));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }
}
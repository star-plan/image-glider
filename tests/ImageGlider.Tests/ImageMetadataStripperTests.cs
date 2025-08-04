using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;

namespace ImageGlider.Tests;

/// <summary>
/// ImageConverter 元数据清理功能的单元测试
/// </summary>
public class ImageMetadataStripperTests
{
    /// <summary>
    /// 测试单文件元数据清理 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void StripMetadata_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.jpg";

        // Act
        var result = ImageConverter.StripMetadata(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试单文件元数据清理 - 成功清理所有元数据
    /// </summary>
    [Fact]
    public void StripMetadata_ValidFile_StripsAllMetadata()
    {
        // Arrange
        var sourceFile = TestImageHelper.CreateTestImageWithMetadata();
        var targetFile = Path.GetTempFileName() + ".jpg";

        try
        {
            // Act
            var result = ImageConverter.StripMetadata(sourceFile, targetFile, stripAll: true);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));

            // 验证元数据已被清理
            using var image = Image.Load(targetFile);
            Assert.Null(image.Metadata.ExifProfile);
            Assert.Null(image.Metadata.IccProfile);
            Assert.Null(image.Metadata.XmpProfile);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupFile(sourceFile);
            TestImageHelper.CleanupFile(targetFile);
        }
    }

    /// <summary>
    /// 测试单文件元数据清理 - 选择性清理EXIF
    /// </summary>
    [Fact]
    public void StripMetadata_ValidFile_StripsOnlyExif()
    {
        // Arrange
        var sourceFile = TestImageHelper.CreateTestImageWithMetadata();
        var targetFile = Path.GetTempFileName() + ".jpg";

        try
        {
            // Act
            var result = ImageConverter.StripMetadata(sourceFile, targetFile, 
                stripAll: false, stripExif: true, stripIcc: false, stripXmp: false);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));

            // 验证只有EXIF被清理
            using var image = Image.Load(targetFile);
            Assert.Null(image.Metadata.ExifProfile);
            // 注意：由于测试图像可能没有ICC和XMP，这里只验证EXIF被清理
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupFile(sourceFile);
            TestImageHelper.CleanupFile(targetFile);
        }
    }

    /// <summary>
    /// 测试批量元数据清理 - 源目录不存在
    /// </summary>
    [Fact]
    public void BatchStripMetadata_SourceDirectoryNotExists_ReturnsFailure()
    {
        // Arrange
        var sourceDirectory = "nonexistent_directory";
        var outputDirectory = Path.GetTempPath();
        var sourceExtension = ".jpg";

        // Act
        var result = ImageConverter.BatchStripMetadata(sourceDirectory, outputDirectory, sourceExtension);

        // Assert
            Assert.False(result.Success);
            Assert.Contains("源目录不存在", result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量元数据清理 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchStripMetadata_ValidDirectory_ProcessesAllFiles()
    {
        // Arrange
        var sourceDirectory = TestImageHelper.CreateTempDirectory();
        var outputDirectory = TestImageHelper.CreateTempDirectory();
        
        // 创建测试图片文件
        var testFiles = new[]
        {
            TestImageHelper.CreateTestImageWithMetadata(Path.Combine(sourceDirectory, "test1.jpg")),
            TestImageHelper.CreateTestImageWithMetadata(Path.Combine(sourceDirectory, "test2.jpg")),
            TestImageHelper.CreateTestImageWithMetadata(Path.Combine(sourceDirectory, "test3.jpg"))
        };

        try
        {
            // Act
            var result = ImageConverter.BatchStripMetadata(sourceDirectory, outputDirectory, ".jpg");

            // Assert
            Assert.True(result.Success);
            Assert.Equal(3, result.TotalFiles);
            Assert.Equal(3, result.SuccessCount);
            Assert.Equal(0, result.FailureCount);

            // 验证输出文件存在
            Assert.True(File.Exists(Path.Combine(outputDirectory, "test1.jpg")));
            Assert.True(File.Exists(Path.Combine(outputDirectory, "test2.jpg")));
            Assert.True(File.Exists(Path.Combine(outputDirectory, "test3.jpg")));

            // 验证元数据已被清理
            using var image = Image.Load(Path.Combine(outputDirectory, "test1.jpg"));
            Assert.Null(image.Metadata.ExifProfile);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(sourceDirectory);
            TestImageHelper.CleanupDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// 测试批量元数据清理 - 空目录
    /// </summary>
    [Fact]
    public void BatchStripMetadata_EmptyDirectory_ReturnsNoFiles()
    {
        // Arrange
        var sourceDirectory = TestImageHelper.CreateTempDirectory();
        var outputDirectory = TestImageHelper.CreateTempDirectory();

        try
        {
            // Act
            var result = ImageConverter.BatchStripMetadata(sourceDirectory, outputDirectory, ".jpg");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("未找到匹配的源文件", result.ErrorMessage);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(sourceDirectory);
            TestImageHelper.CleanupDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// 测试元数据清理 - 验证质量参数
    /// </summary>
    [Fact]
    public void StripMetadata_WithQuality_PreservesQuality()
    {
        // Arrange
        var sourceFile = TestImageHelper.CreateTestImageWithMetadata();
        var targetFile = Path.GetTempFileName() + ".jpg";
        var quality = 75;

        try
        {
            // Act
            var result = ImageConverter.StripMetadata(sourceFile, targetFile, quality: quality);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));

            // 验证文件大小合理（质量较低应该文件更小）
            var originalSize = new FileInfo(sourceFile).Length;
            var processedSize = new FileInfo(targetFile).Length;
            
            // 处理后的文件应该更小（因为清理了元数据且质量较低）
            Assert.True(processedSize <= originalSize);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupFile(sourceFile);
            TestImageHelper.CleanupFile(targetFile);
        }
    }
}
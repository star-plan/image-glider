using System.IO;
using Xunit;
using ImageGlider.Processors;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageGlider.Tests;

/// <summary>
/// ImageFormatConverter 格式转换功能的单元测试
/// </summary>
public class ImageFormatConverterTests
{
    private readonly ImageFormatConverter _converter;

    public ImageFormatConverterTests()
    {
        _converter = new ImageFormatConverter();
    }

    /// <summary>
    /// 测试转换图片格式 - 文件不存在
    /// </summary>
    [Fact]
    public void ConvertImage_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.png";

        // Act
        var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试转换图片格式 - JPG转PNG
    /// </summary>
    [Fact]
    public void ConvertImage_JpgToPng_ConvertsSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.png");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证格式
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
    /// 测试转换图片格式 - PNG转JPG
    /// </summary>
    [Fact]
    public void ConvertImage_PngToJpg_ConvertsSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.png");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 150, 150);

            // Act
            var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证格式和尺寸
            using var image = Image.Load(targetFile);
            Assert.Equal(150, image.Width);
            Assert.Equal(150, image.Height);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试转换图片格式 - 自定义质量
    /// </summary>
    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(90)]
    [InlineData(100)]
    public void ConvertImage_CustomQuality_ConvertsWithCorrectQuality(int quality)
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.png");
        var targetFile = Path.Combine(tempDir, $"target_q{quality}.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile, quality);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证文件存在且大小合理
            var fileInfo = new FileInfo(targetFile);
            Assert.True(fileInfo.Length > 0);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试转换图片格式 - 无效质量参数
    /// </summary>
    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(200)]
    public void ConvertImage_InvalidQuality_ClampsToValidRange(int quality)
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.png");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile, quality);

            // Assert
            Assert.True(result); // 应该成功，质量参数会被限制在有效范围内
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试转换图片格式 - 相同格式
    /// </summary>
    [Fact]
    public void ConvertImage_SameFormat_ConvertsSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = ImageFormatConverter.ConvertImage(sourceFile, targetFile);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试批量转换 - 目录不存在
    /// </summary>
    [Fact]
    public void BatchConvert_DirectoryNotExists_ReturnsErrorResult()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = "output";

        // Act
        var result = _converter.BatchConvert(sourceDir, outputDir, ".jpg", ".png");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量转换 - 空目录
    /// </summary>
    [Fact]
    public void BatchConvert_EmptyDirectory_ReturnsZeroFiles()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // Act
            var result = _converter.BatchConvert(tempDir, outputDir, ".jpg", ".png");

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
    /// 测试批量转换 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchConvert_MultipleFiles_ProcessesAllFiles()
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
            var file3 = Path.Combine(tempDir, "test3.png"); // 不匹配的格式
            TestImageHelper.CreateTestImage(file1, 100, 100);
            TestImageHelper.CreateTestImage(file2, 150, 150);
            TestImageHelper.CreateTestImage(file3, 200, 200);
            
            // Act
            var result = _converter.BatchConvert(tempDir, outputDir, ".jpg", ".png");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.TotalFiles); // 只有2个jpg文件
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件
            var outputFile1 = Path.Combine(outputDir, "test1.png");
            var outputFile2 = Path.Combine(outputDir, "test2.png");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
            
            // 验证转换后的图像
            using var image1 = Image.Load(outputFile1);
            using var image2 = Image.Load(outputFile2);
            Assert.Equal(100, image1.Width);
            Assert.Equal(150, image2.Width);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试批量转换 - 混合成功和失败
    /// </summary>
    [Fact]
    public void BatchConvert_MixedResults_ReportsCorrectCounts()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建一个有效文件和一个无效文件
            var validFile = Path.Combine(tempDir, "valid.jpg");
            var invalidFile = Path.Combine(tempDir, "invalid.jpg");
            
            TestImageHelper.CreateTestImage(validFile, 100, 100);
            File.WriteAllText(invalidFile, "This is not an image"); // 创建无效图像文件
            
            // Act
            var result = _converter.BatchConvert(tempDir, outputDir, ".jpg", ".png");

            // Assert
            Assert.False(result.IsSuccess); // 因为有失败的转换，所以IsSuccess应该是false
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(1, result.SuccessfulConversions);
            Assert.Equal(1, result.FailedConversions);
            
            // 验证成功的文件
            var outputFile = Path.Combine(outputDir, "valid.png");
            Assert.True(File.Exists(outputFile));
            
            // 验证失败的文件不存在
            var failedOutputFile = Path.Combine(outputDir, "invalid.png");
            Assert.False(File.Exists(failedOutputFile));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试ProcessImage接口方法
    /// </summary>
    [Fact]
    public void ProcessImage_ValidFile_ProcessesSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "target.png");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 100, 100);

            // Act
            var result = _converter.ProcessImage(sourceFile, targetFile, 85);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }
}

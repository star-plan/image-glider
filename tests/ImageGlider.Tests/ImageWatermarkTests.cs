using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Enums;
using ImageGlider.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageGlider.Tests;

/// <summary>
/// ImageWatermark 水印功能的单元测试
/// </summary>
public class ImageWatermarkTests
{
    /// <summary>
    /// 测试添加文本水印 - 文件不存在的情况
    /// </summary>
    [Fact]
    public void AddTextWatermark_FileNotExists_ReturnsFalse()
    {
        // Arrange
        var sourceFile = "nonexistent.jpg";
        var targetFile = "output.jpg";
        var text = "Test Watermark";

        // Act
        var result = ImageConverter.AddTextWatermark(sourceFile, targetFile, text);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 测试添加文本水印 - 成功添加
    /// </summary>
    [Fact]
    public void AddTextWatermark_ValidFile_AddsWatermarkSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "watermarked.jpg");
        var text = "Test Watermark";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.AddTextWatermark(sourceFile, targetFile, text);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(targetFile));
            
            // 验证文件大小合理
            var originalSize = new FileInfo(sourceFile).Length;
            var watermarkedSize = new FileInfo(targetFile).Length;
            Assert.True(watermarkedSize > 0);
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
        }
    }

    /// <summary>
    /// 测试添加文本水印 - 不同位置
    /// </summary>
    [Theory]
    [InlineData(WatermarkPosition.TopLeft)]
    [InlineData(WatermarkPosition.TopCenter)]
    [InlineData(WatermarkPosition.TopRight)]
    [InlineData(WatermarkPosition.MiddleLeft)]
    [InlineData(WatermarkPosition.Center)]
    [InlineData(WatermarkPosition.MiddleRight)]
    [InlineData(WatermarkPosition.BottomLeft)]
    [InlineData(WatermarkPosition.BottomCenter)]
    [InlineData(WatermarkPosition.BottomRight)]
    public void AddTextWatermark_DifferentPositions_AddsWatermarkSuccessfully(WatermarkPosition position)
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, $"watermarked_{position}.jpg");
        var text = "Test";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.AddTextWatermark(sourceFile, targetFile, text, position);

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
    /// 测试添加文本水印 - 自定义参数
    /// </summary>
    [Fact]
    public void AddTextWatermark_CustomParameters_AddsWatermarkSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "watermarked.jpg");
        var text = "Custom Watermark";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 300, 300);

            // Act - 使用自定义参数
            var result = ImageConverter.AddTextWatermark(
                sourceFile, 
                targetFile, 
                text, 
                WatermarkPosition.BottomRight,
                fontSize: 24,
                fontColor: "red",
                opacity: 80,
                quality: 95
            );

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
    /// 测试添加图片水印 - 水印文件不存在
    /// </summary>
    [Fact]
    public void AddImageWatermark_WatermarkFileNotExists_ReturnsFalse()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var targetFile = Path.Combine(tempDir, "watermarked.jpg");
        var watermarkFile = "nonexistent_watermark.png";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 200, 200);

            // Act
            var result = ImageConverter.AddImageWatermark(sourceFile, targetFile, watermarkFile);

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
    /// 测试添加图片水印 - 成功添加
    /// </summary>
    [Fact]
    public void AddImageWatermark_ValidFiles_AddsWatermarkSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var watermarkFile = Path.Combine(tempDir, "watermark.png");
        var targetFile = Path.Combine(tempDir, "watermarked.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 300, 300);
            TestImageHelper.CreateTestImage(watermarkFile, 50, 50, Color.Red);

            // Act
            var result = ImageConverter.AddImageWatermark(sourceFile, targetFile, watermarkFile);

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
    /// 测试添加图片水印 - 自定义参数
    /// </summary>
    [Fact]
    public void AddImageWatermark_CustomParameters_AddsWatermarkSuccessfully()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var sourceFile = Path.Combine(tempDir, "source.jpg");
        var watermarkFile = Path.Combine(tempDir, "watermark.png");
        var targetFile = Path.Combine(tempDir, "watermarked.jpg");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            TestImageHelper.CreateTestImage(sourceFile, 400, 400);
            TestImageHelper.CreateTestImage(watermarkFile, 80, 80, Color.Blue);

            // Act - 使用自定义参数
            var result = ImageConverter.AddImageWatermark(
                sourceFile, 
                targetFile, 
                watermarkFile,
                WatermarkPosition.TopLeft,
                opacity: 30,
                scale: 0.5f,
                quality: 85
            );

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
    /// 测试批量添加文本水印 - 目录不存在
    /// </summary>
    [Fact]
    public void BatchAddTextWatermark_DirectoryNotExists_ReturnsErrorResult()
    {
        // Arrange
        var sourceDir = "nonexistent_directory";
        var outputDir = "output";
        var text = "Batch Watermark";

        // Act
        var result = ImageConverter.BatchAddTextWatermark(sourceDir, outputDir, ".jpg", text);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    /// <summary>
    /// 测试批量添加文本水印 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchAddTextWatermark_MultipleFiles_ProcessesAllFiles()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        var text = "Batch Test";
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建测试文件
            var file1 = Path.Combine(tempDir, "test1.jpg");
            var file2 = Path.Combine(tempDir, "test2.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);
            TestImageHelper.CreateTestImage(file2, 250, 250);
            
            // Act
            var result = ImageConverter.BatchAddTextWatermark(tempDir, outputDir, ".jpg", text);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件
            var outputFile1 = Path.Combine(outputDir, "test1_watermarked.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2_watermarked.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }

    /// <summary>
    /// 测试批量添加图片水印 - 成功处理多个文件
    /// </summary>
    [Fact]
    public void BatchAddImageWatermark_MultipleFiles_ProcessesAllFiles()
    {
        // Arrange
        var tempDir = TestImageHelper.CreateTempDirectory();
        var outputDir = TestImageHelper.CreateTempDirectory();
        var watermarkFile = Path.Combine(tempDir, "watermark.png");
        
        try
        {
            Directory.CreateDirectory(tempDir);
            
            // 创建水印文件和测试文件
            TestImageHelper.CreateTestImage(watermarkFile, 30, 30, Color.Yellow);
            var file1 = Path.Combine(tempDir, "test1.jpg");
            var file2 = Path.Combine(tempDir, "test2.jpg");
            TestImageHelper.CreateTestImage(file1, 200, 200);
            TestImageHelper.CreateTestImage(file2, 250, 250);
            
            // Act
            var result = ImageConverter.BatchAddImageWatermark(tempDir, outputDir, ".jpg", watermarkFile);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.TotalFiles);
            Assert.Equal(2, result.SuccessfulConversions);
            Assert.Equal(0, result.FailedConversions);
            
            // 验证输出文件
            var outputFile1 = Path.Combine(outputDir, "test1_watermarked.jpg");
            var outputFile2 = Path.Combine(outputDir, "test2_watermarked.jpg");
            Assert.True(File.Exists(outputFile1));
            Assert.True(File.Exists(outputFile2));
        }
        finally
        {
            // Cleanup
            TestImageHelper.CleanupDirectory(tempDir);
            TestImageHelper.CleanupDirectory(outputDir);
        }
    }
}

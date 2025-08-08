using System.IO;
using Xunit;
using ImageGlider;
using ImageGlider.Tests.TestHelpers;
using ImageGlider.Core;
using ImageGlider.Enums;

namespace ImageGlider.Tests;

/// <summary>
/// 批量处理功能的单元测试
/// </summary>
public class BatchProcessingTests : IDisposable
{
    private readonly string _tempSourceDir;
    private readonly string _tempOutputDir;
    
    public BatchProcessingTests()
    {
        _tempSourceDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _tempOutputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempSourceDir);
        Directory.CreateDirectory(_tempOutputDir);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_tempSourceDir))
            Directory.Delete(_tempSourceDir, true);
        if (Directory.Exists(_tempOutputDir))
            Directory.Delete(_tempOutputDir, true);
    }
    
    [Fact]
    public void BatchConvert_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 100, 100);
        TestImageHelper.CreateTestImage(sourceFile2, 200, 200);
        
        // Act
        var result = ImageConverter.BatchConvert(_tempSourceDir, _tempOutputDir, ".jpg", ".png", 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.Equal(2, result.SuccessfulFiles.Count);
        Assert.Empty(result.FailedFiles);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.png")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.png")));
    }
    
    [Fact]
    public void BatchConvert_WithNonExistentSourceDirectory_ShouldReturnFailureResult()
    {
        // Arrange
        var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        // Act
        var result = ImageConverter.BatchConvert(nonExistentDir, _tempOutputDir, ".jpg", ".png", 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("源目录不存在", result.ErrorMessage);
    }
    
    [Fact]
    public void BatchConvert_WithEmptyDirectory_ShouldReturnEmptyResult()
    {
        // Act
        var result = ImageConverter.BatchConvert(_tempSourceDir, _tempOutputDir, ".jpg", ".png", 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        Assert.Empty(result.SuccessfulFiles);
        Assert.Empty(result.FailedFiles);
    }
    
    [Fact]
    public void BatchResize_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 400, 400);
        TestImageHelper.CreateTestImage(sourceFile2, 600, 600);
        
        // Act
        var result = ImageConverter.BatchResize(_tempSourceDir, _tempOutputDir, ".jpg", 200, 200, ResizeMode.KeepAspectRatio, 85);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Fact]
    public void BatchCompress_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 300, 300);
        TestImageHelper.CreateTestImage(sourceFile2, 400, 400);
        
        // Act
        var result = ImageConverter.BatchCompress(_tempSourceDir, _tempOutputDir, ".jpg", 70);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Fact]
    public void BatchStripMetadata_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 200, 200);
        TestImageHelper.CreateTestImage(sourceFile2, 300, 300);
        
        // Act
        var result = ImageConverter.BatchStripMetadata(_tempSourceDir, _tempOutputDir, ".jpg", true, true, false, true, 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.Equal(2, result.ProcessedFiles.Count);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Fact]
    public void BatchCrop_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 400, 400);
        TestImageHelper.CreateTestImage(sourceFile2, 500, 500);
        
        // Act
        var result = ImageConverter.BatchCrop(_tempSourceDir, _tempOutputDir, ".jpg", 50, 50, 200, 200, 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Fact]
    public void BatchAddTextWatermark_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 300, 300);
        TestImageHelper.CreateTestImage(sourceFile2, 400, 400);
        
        // Act
        var result = ImageConverter.BatchAddTextWatermark(_tempSourceDir, _tempOutputDir, ".jpg", "Test Watermark", WatermarkPosition.BottomRight, 50, 24, "#FFFFFF", 85);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Fact]
    public void BatchAdjustColor_WithValidFiles_ShouldReturnSuccessResult()
    {
        // Arrange
        var sourceFile1 = Path.Combine(_tempSourceDir, "test1.jpg");
        var sourceFile2 = Path.Combine(_tempSourceDir, "test2.jpg");
        
        TestImageHelper.CreateTestImage(sourceFile1, 200, 200);
        TestImageHelper.CreateTestImage(sourceFile2, 250, 250);
        
        // Act
        var result = ImageConverter.BatchAdjustColor(_tempSourceDir, _tempOutputDir, ".jpg", 10, 5, 15, 0, 1.0f, 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(2, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test1.jpg")));
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "test2.jpg")));
    }
    
    [Theory]
    [InlineData(".jpg", ".png")]
    [InlineData(".png", ".jpg")]
    [InlineData(".jpeg", ".webp")]
    [InlineData(".bmp", ".png")]
    public void BatchConvert_DifferentFormats_ShouldWork(string sourceExt, string targetExt)
    {
        // Arrange
        var sourceFile = Path.Combine(_tempSourceDir, $"test{sourceExt}");
        TestImageHelper.CreateTestImage(sourceFile, 150, 150);
        
        // Act
        var result = ImageConverter.BatchConvert(_tempSourceDir, _tempOutputDir, sourceExt, targetExt, 85);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.TotalFiles);
        Assert.Equal(1, result.SuccessfulConversions);
        Assert.Equal(0, result.FailedConversions);
        
        // 验证输出文件存在
        var expectedOutputFile = Path.Combine(_tempOutputDir, $"test{targetExt}");
        Assert.True(File.Exists(expectedOutputFile));
    }
    
    [Fact]
    public void BatchConvert_WithMixedValidAndInvalidFiles_ShouldPartiallySucceed()
    {
        // Arrange
        var validFile = Path.Combine(_tempSourceDir, "valid.jpg");
        var invalidFile = Path.Combine(_tempSourceDir, "invalid.jpg");
        
        TestImageHelper.CreateTestImage(validFile, 100, 100);
        File.WriteAllText(invalidFile, "This is not an image file");
        
        // Act
        var result = ImageConverter.BatchConvert(_tempSourceDir, _tempOutputDir, ".jpg", ".png", 90);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess); // 因为有失败的文件
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(1, result.SuccessfulConversions);
        Assert.Equal(1, result.FailedConversions);
        Assert.Single(result.SuccessfulFiles);
        Assert.Single(result.FailedFiles);
        
        // 验证成功的文件存在
        Assert.True(File.Exists(Path.Combine(_tempOutputDir, "valid.png")));
        // 验证失败的文件不存在
        Assert.False(File.Exists(Path.Combine(_tempOutputDir, "invalid.png")));
    }
}
